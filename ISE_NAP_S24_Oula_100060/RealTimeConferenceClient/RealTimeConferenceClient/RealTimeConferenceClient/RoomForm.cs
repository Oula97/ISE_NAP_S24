using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using NAudio.Wave;
using NAudio.Wave.Compression;


namespace RealTimeConferenceClient
{
    public partial class RoomForm : Form
    {
        private ClientWebSocket webSocketClient;
        private Room currentRoom;
        private string roomName;
        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        private const int AudioPort = 12346; // Add this line
        private UdpClient udpClient = new UdpClient("127.0.0.1", 12345);
        private const string ServerAddress = "127.0.0.1";
        private const int Port = 5000;
        private Uri serverUri = new Uri("ws://127.0.0.1:8081");
        private Process serverProcess;
        private TcpListener tcpListener;
        private UdpClient udpServer;
        private UdpClient videoUdpClient;
        private UdpClient audioUdpClient;
        private const int VideoPort = 5000;
        private DateTime lastFrameTime = DateTime.Now;
        private Dictionary<string, PictureBox> memberVideoBoxes = new Dictionary<string, PictureBox>();
        private WebSocketClient _webSocketClient;
        private UdpStreamingClient _udpClient; // تعديل هنا
        private TcpChatClient _tcpChatClient;
        private Thread tcpThread;
        private Thread udpThread;
        private Thread websocketThread;
        private CancellationTokenSource cancellationTokenSource;
        private HttpListener _httpListener;
        private List<WebSocket> _webSockets = new List<WebSocket>();
        private UdpVideoAudioManager videoAudioManager;
        private VideoCaptureDevice videoSource; // كاميرا الفيديو
        private WaveInEvent audioSource; // مدخل الصوت
        private WaveOutEvent audioOutput; // إخراج الصوت
        private BufferedWaveProvider bufferedWaveProvider;
        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;


        public RoomForm(Room room)
        {
            InitializeComponent();
            currentRoom = room;
            lblRoomName.Text = $"Active Rooms: {currentRoom.Name}";
            lblMemberCount.Text = $"Members: {currentRoom.MemberCount}";
            _webSocketClient = new WebSocketClient();
            _tcpChatClient = new TcpChatClient();
            _udpClient = new UdpStreamingClient(7000); // منفذ العميل المحلي
        }
        private void RoomForm_Load(object sender, EventArgs e)
        {
            // استدعاء StartServer عند تحميل النموذج
            StartServer();
            _webSocketClient.ConnectAsync("ws://localhost:8080/"); // اتصال WebSocket
            _tcpChatClient.ConnectAsync("localhost", 5000); // عنوان ونظام دردشة
            _udpClient.StartReceiving(); // بدء استقبال البث

        }
        private void CaptureAndSendAudio()
        {
            audioSource = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            audioSource.DataAvailable += WaveIn_DataAvailable;

            audioSource.StartRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (bufferedWaveProvider != null)
            {
                bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);

                // إرسال البيانات الصوتية عبر UDP
                udpClient.Send(e.Buffer, e.BytesRecorded);
            }
        }
        private void StartAudioCapture()
        {
            if (waveIn == null)
            {
                MessageBox.Show("WaveIn not initialized.");
                return;
            }

            waveIn.StartRecording();
            waveOut.Play();
        }

        private async void StartServer()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            // بدء خادم TCP في خيط منفصل
            tcpThread = new Thread(() => StartTcpServer(token))
            {
                IsBackground = true
            };
            tcpThread.Start();
            textBoxConnectionStatus.Text = "Connected via TCP ,";
            // بدء خادم UDP في خيط منفصل
            udpThread = new Thread(() => StartUdpServer(token))
            {
                IsBackground = true
            };
            udpThread.Start();
            textBoxConnectionStatus.Text += " UDP ,";
            // بدء خادم WebSocket في خيط منفصل
            websocketThread = new Thread(async () => await StartWebSocketServer(token))
            {
                IsBackground = true
            };
            websocketThread.Start();
            textBoxConnectionStatus.Text += " Websocket.";
            MessageBox.Show("All servers are running successfully!", "Server Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void StartTcpServer(CancellationToken token)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 5000);
                tcpListener.Start();

                while (!token.IsCancellationRequested)
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        _ = Task.Run(() => HandleTcpClient(client, token));
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                LogError("TCP Server Error", ex);
                Invoke(new Action(() => textBoxConnectionStatus.Text = $"TCP Server Error: {ex.Message}"));
            }
        }
        private void LogError(string message, Exception ex)
        {
            // Log the error to a file or monitoring system
            Debug.WriteLine($"{message}: {ex}");
        }
        private async Task HandleTcpClient(TcpClient client, CancellationToken token)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                // الاستماع للرسائل الواردة من العميل
                while (client.Connected && !token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received from client: {message}");

                        // إرسال رد إلى العميل
                        string response = $"Received: {message}";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    else
                    {
                        break; // العميل أغلق الاتصال
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
        private void StartUdpServer(CancellationToken token)
        {
            try
            {
                // تهيئة UdpStreamingClient بدلاً من UdpClient
                _udpClient = new UdpStreamingClient(Port);

                // البدء في استقبال البيانات باستخدام StartReceiving
                _udpClient.StartReceiving();

                // لوضع إضافي (اختياري)، يمكنك إرسال رسالة إلى العميل بعد بدء الخادم
                _udpClient.SendAsync("localhost", Port, "UDP Server Started!").Wait();

                //// إخبار المستخدم بأن الخادم قد بدأ
                //Invoke(new Action(() => textBoxConnectionStatus.Text += "\nUDP Server started successfully."));

                // الانتظار لطلب إنهاء الخدمة
                while (!token.IsCancellationRequested)
                {
                    // انتظار حتى يتم إلغاء الطلب
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء مثل فشل الاتصال
                Invoke(new Action(() => textBoxConnectionStatus.Text = $"UDP Server Error: {ex.Message}"));
            }
        }
        private void ReceiveTcpData()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = tcpStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //Invoke(new Action(() => listBoxMessages.Items.Add("TCP: " + message)));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => textBoxConnectionStatus.Text = $"TCP Error: {ex.Message}"));
            }
        }
        private async Task SendTcpData(string message)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(message);
            await tcpStream.WriteAsync(sendData, 0, sendData.Length);
            listBoxMessages.Items.Add($"You {currentRoom.MemberCount}: {message}");
        }
        private async Task SendUdpData(string message)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(sendData, sendData.Length);
            listBoxMessages.Items.Add("You: " + message);
        }
        private void ReceiveUdpData()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
                while (true)
                {
                    byte[] receiveData = udpClient.Receive(ref endPoint);
                    if (receiveData[0] == 0x03)
                    {
                        // Handle message
                        string message = Encoding.UTF8.GetString(receiveData.Skip(1).ToArray());
                        // Display message or handle accordingly
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => textBoxConnectionStatus.Text = $"UDP Error: {ex.Message}"));
            }
        }
        private async Task ReceiveMessages(ClientWebSocket client, CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            while (!token.IsCancellationRequested && client.State == WebSocketState.Open)
            {
                try
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Invoke(new Action(() =>
                        {
                            listBoxMessages.Items.Add($"Server: {message}");
                        }));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Invoke(new Action(() =>
                        {
                            textBoxConnectionStatus.Text = "Server closed the connection.";
                        }));
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        textBoxConnectionStatus.Text = $"Error receiving messages: {ex.Message}";
                    }));
                    break;
                }
            }
        }
        private async Task ReceiveWebSocketData()
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (webSocketClient != null && webSocketClient.State == WebSocketState.Open)
                {
                    var result = await webSocketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        Invoke(new Action(() =>
                        {
                            textBoxConnectionStatus.Text = "WebSocket connection closed.";
                        }));
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    Invoke(new Action(() =>
                    {
                        listBoxMessages.Items.Add("WebSocket: " + message);
                    }));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    textBoxConnectionStatus.Text = $"WebSocket Receive Error: {ex.Message}";
                }));
            }
        }
        private async Task StartWebSocketServer(CancellationToken token)
        {
            try
            {
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add("http://localhost:8080/"); // تحديد العنوان والمنفذ
                _httpListener.Start();
                Console.WriteLine("WebSocket Server Started.");

                while (!token.IsCancellationRequested)
                {
                    // انتظر حتى يأتي طلب WebSocket
                    HttpListenerContext context = await _httpListener.GetContextAsync();

                    if (context.Request.IsWebSocketRequest)
                    {
                        // قبول طلب WebSocket
                        WebSocket webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                        _webSockets.Add(webSocket);
                        Console.WriteLine("WebSocket connection established.");

                        // الاستماع للرسائل الواردة من WebSocket
                        _ = Task.Run(() => ListenWebSocketAsync(webSocket, token));
                    }
                    else
                    {
                        context.Response.StatusCode = 400; // طلب غير صحيح
                        context.Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Server Error: {ex.Message}");
            }
        }
        private async Task ListenWebSocketAsync(WebSocket webSocket, CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received from WebSocket client: {message}");

                        // إرسال رد للعميل (اختياري)
                        string response = $"Received: {message}";
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Error: {ex.Message}");
            }
            finally
            {
                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();
                _webSockets.Remove(webSocket);
                Console.WriteLine("WebSocket connection closed.");
            }
        }
        private async Task HandleWebSocketClient(WebSocket websocket, CancellationToken token)
        {
            try
            {
                var buffer = new byte[1024];
                var segment = new ArraySegment<byte>(buffer);

                while (!token.IsCancellationRequested)
                {
                    var result = await websocket.ReceiveAsync(segment, token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received WebSocket message: {message}");

                        // يمكنك إرسال رد للعميل إذا لزم الأمر
                        byte[] responseBytes = Encoding.UTF8.GetBytes("Message received");
                        await websocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, token);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", token);
                        Console.WriteLine("WebSocket connection closed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling WebSocket client: {ex.Message}");
            }
            finally
            {
                websocket.Dispose();
            }
        }
        private void CleanupResources()
        {
           
            videoUdpClient?.Close();
            audioUdpClient?.Close();
        }
        private volatile bool isStreaming = false;

        private void StopVideoAndAudioTasks()
        {
            isStreaming = false; // إيقاف استقبال البيانات
            Thread.Sleep(100); // تأخير بسيط للتأكد من انتهاء الخيوط
        }


        private async void buttonSend_Click(object sender, EventArgs e)
        {
            string message = textBoxMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Cannot send an empty message.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            textBoxMessage.Clear();

            await _tcpChatClient.SendMessageAsync(message);
            await _webSocketClient.SendAsync(message);

            listBoxMessages.Items.Add($"You: {message}");
        }
        private void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            // عرض رسالة تأكيد قبل الإغلاق
            DialogResult result = MessageBox.Show(
                "Are you sure you want to leave this Room?",
                "Leave Room",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result == DialogResult.Yes)
            {
                MainDashboard mainDashboard = new MainDashboard();
                mainDashboard.Show();
                this.Close();
            }

        }
        private void btnSendVideo_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            buttonstopvideo.Visible = true;
            panelVideo.Visible = true;
            flowLayoutPanel1.Visible = true;
            InitializeVideo();
            InitializeAudio();
            StartAudioCapture();
            // تشغيل استقبال الفيديو في خيط منفصل
            Thread videoReceiveThread = new Thread(ReceiveVideoStream)
            {
                IsBackground = true
            };
            videoReceiveThread.Start();
        }
        private void InitializeVideo()
        {
            try
            {
                VideoCaptureDeviceForm captureDeviceForm = new VideoCaptureDeviceForm();
                if (captureDeviceForm.ShowDialog() == DialogResult.OK)
                {
                    videoSource = captureDeviceForm.VideoDevice;
                    videoSource.NewFrame += VideoSource_NewFrame;
                    videoSource.Start();
                    Debug.WriteLine("Video source started successfully.");
                }
                else
                {
                    Debug.WriteLine("No video device selected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing video: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error initializing video: {ex.Message}");
            }
        }
        private void InitializeAudio()
        {
            try
            {
                waveIn = new WaveInEvent();
                waveIn.WaveFormat = new WaveFormat(44100, 1);
                waveIn.DataAvailable += WaveIn_DataAvailable;

                bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);

                waveOut = new WaveOutEvent();
                waveOut.Init(bufferedWaveProvider);

                udpClient = new UdpClient();
                udpClient.Connect(ServerAddress, AudioPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing audio: {ex.Message}");
            }
        }
        private void ReceiveAudioStream()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Any, AudioPort);
                while (true)
                {
                    if (audioUdpClient == null)
                    {
                        Debug.WriteLine("audioUdpClient is not initialized.");
                        return;
                    }

                    byte[] data = audioUdpClient.Receive(ref endPoint);
                    bufferedWaveProvider.AddSamples(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error receiving audio data: {ex.Message}");
            }
        }
        private async Task BroadcastMessageToRoom(string message, Room room)
        {
            foreach (var socket in room.WebSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private async Task NotifyRoomUpdate(string updateMessage, Room room)
        {
            foreach (var socket in room.WebSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(updateMessage);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        private void ReceiveVideoStream()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Any, VideoPort);
                while (true)
                {
                    if (videoUdpClient == null)
                    {
                        Debug.WriteLine("videoUdpClient is not initialized.");
                        return;
                    }

                    byte[] data = videoUdpClient.Receive(ref endPoint);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        Bitmap frame = new Bitmap(ms);
                        panelVideo.Invoke(new Action(() =>
                        {
                            using (Graphics g = panelVideo.CreateGraphics())
                            {
                                g.DrawImage(frame, 0, 0, panelVideo.Width, panelVideo.Height);
                            }
                        }));
                        Debug.WriteLine("Video frame received and displayed successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error receiving video frame: {ex.Message}");
            }
        }
        private void buttonstopvideo_Click(object sender, EventArgs e)
        {
            try
            {
                // إيقاف مصدر الفيديو
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop(); // إرسال إشارة لإيقاف الكاميرا
                    videoSource.WaitForStop(); // انتظار التوقف الكامل
                    videoSource = null; // تحرير المورد
                    MessageBox.Show ("Video source stopped successfully.");
                }
                // إيقاف التسجيل
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    waveIn.DataAvailable -= WaveIn_DataAvailable; // إزالة الاشتراك في الحدث
                    waveIn.Dispose();
                    waveIn = null; // تحرير الموارد
                }

                // إيقاف تشغيل الصوت
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null; // تحرير الموارد
                    MessageBox.Show("Audio source stopped successfully.");

                }

                // حذف المخزن المؤقت للصوت
                bufferedWaveProvider = null;
            

                // إخفاء العناصر من الواجهة
                panel1.Visible = true;
                buttonstopvideo.Visible = false;
                flowLayoutPanel1.Visible = false;
                panelVideo.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping video/audio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error stopping video/audio: {ex.Message}");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // عرض إطار الفيديو على Panel
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                using (Graphics g = panelVideo.CreateGraphics())
                {
                    g.DrawImage(frame, 0, 0, panelVideo.Width, panelVideo.Height);
                }

                // تحويل الإطار إلى بيانات ByteArray
                using (MemoryStream ms = new MemoryStream())
                {
                    frame.Save(ms, ImageFormat.Jpeg);
                    byte[] frameData = ms.ToArray();

                    // إرسال الإطار عبر UDP
                    udpClient.Send(frameData, frameData.Length);
                    Debug.WriteLine("Video frame sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending video frame: {ex.Message}");
            }
        }


        private void SendVideoFrame(Bitmap frame)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                frame.Save(ms, ImageFormat.Jpeg);
                byte[] frameData = ms.ToArray();

                const int maxChunkSize = 64000; // UDP limit minus headers
                for (int i = 0; i < frameData.Length; i += maxChunkSize)
                {
                    int chunkSize = Math.Min(maxChunkSize, frameData.Length - i);
                    byte[] chunk = new byte[chunkSize];
                    Array.Copy(frameData, i, chunk, 0, chunkSize);
                    videoUdpClient.Send(chunk, chunk.Length, ServerAddress, VideoPort);
                }
            }
        }
        private void AudioSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (audioUdpClient != null)
            {
                // إرسال البيانات الصوتية عبر UDP
                audioUdpClient.Send(e.Buffer, e.BytesRecorded);
            }
            else
            {
                Debug.WriteLine("audioUdpClient is null.");
            }
        }
    }
}

