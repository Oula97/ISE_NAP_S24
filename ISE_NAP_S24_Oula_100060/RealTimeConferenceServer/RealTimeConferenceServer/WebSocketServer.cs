using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeConferenceServer
{
    public class WebSocketServer
    {
        private readonly HttpListener _listener;

        public WebSocketServer(string url)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine("WebSocket Server started...");
            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    WebSocket socket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                    _ = Task.Run(() => HandleClient(socket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClient(WebSocket socket)
        {
            byte[] buffer = new byte[1024];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");

                // Echo message back
                string response = $"Echo: {message}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
