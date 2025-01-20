using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using NAudio.Wave;
using NAudio.Wave.Compression;

namespace RealTimeConferenceClient
{
    internal class UdpVideoAudioManager
    {
        private const string ServerAddress = "127.0.0.1";
        private const int VideoPort = 5000;
        private const int AudioPort = 5000;
        private VideoCapture videoCapture;
        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;
        private BufferedWaveProvider waveProvider;
        private BufferedWaveProvider bufferedWaveProvider;
        private UdpClient videoUdpClient;
        private UdpClient audioUdpClient;

        private volatile bool isStreaming = false;
        private DateTime lastFrameTime = DateTime.Now;

        public event Action<Bitmap> VideoFrameReceived;
        public event Action<byte[]> AudioDataReceived;

        public UdpVideoAudioManager()
        {
            videoUdpClient = new UdpClient();
            audioUdpClient = new UdpClient();

            InitializeAudio();
        }

        public void StartStreaming()
        {
            isStreaming = true;

            Task.Run(CaptureAndSendVideo);
            Task.Run(CaptureAndSendAudio);
            Task.Run(ReceiveVideo);
            Task.Run(ReceiveAudio);
        }

        public void StopStreaming()
        {
            isStreaming = false;

            videoCapture?.Release();
            videoUdpClient?.Close();
            audioUdpClient?.Close();

            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveOut?.Stop();
            waveOut?.Dispose();
        }

        private void CaptureAndSendVideo()
        {
            videoCapture = new VideoCapture(0);
            if (!videoCapture.IsOpened)
                throw new Exception("Cannot access the camera.");

            Mat frame = new Mat();
            while (isStreaming)
            {
                videoCapture.Read(frame);
                if (frame.IsEmpty)
                    continue;

                using (var ms = new MemoryStream())
                {
                    Bitmap bmp = frame.ToBitmap();
                    bmp.Save(ms, ImageFormat.Jpeg);
                    byte[] videoData = ms.ToArray();

                    byte[] packet = new byte[videoData.Length + 1];
                    packet[0] = 0x01;
                    Array.Copy(videoData, 0, packet, 1, videoData.Length);

                    videoUdpClient.Send(packet, packet.Length, ServerAddress, VideoPort);
                }

                var timeDifference = DateTime.Now - lastFrameTime;
                int sleepTime = Math.Max(0, 10 - (int)timeDifference.TotalMilliseconds);
                Thread.Sleep(sleepTime);
                lastFrameTime = DateTime.Now;
            }
        }




        private void CaptureAndSendAudio()
        {
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            waveIn.DataAvailable += (s, e) =>
            {
                byte[] audioData = e.Buffer.Take(e.BytesRecorded).ToArray();
                byte[] packet = new byte[audioData.Length + 1];
                packet[0] = 0x02;
                Array.Copy(audioData, 0, packet, 1, audioData.Length);

                audioUdpClient.Send(packet, packet.Length, ServerAddress, AudioPort);
            };

            waveIn.StartRecording();
        }

        private void ReceiveVideo()
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, VideoPort);
            while (isStreaming)
            {
                byte[] data = videoUdpClient.Receive(ref remoteEndPoint);
                if (data[0] == 0x01)
                {
                    byte[] videoData = data.Skip(1).ToArray();
                    using (var ms = new MemoryStream(videoData))
                    {
                        Bitmap receivedImage = (Bitmap)Image.FromStream(ms);
                        VideoFrameReceived?.Invoke(receivedImage);
                    }
                }
            }
        }

        private void ReceiveAudio()
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, AudioPort);
            while (isStreaming)
            {
                byte[] data = audioUdpClient.Receive(ref remoteEndPoint);
                if (data[0] == 0x02)
                {
                    byte[] audioData = data.Skip(1).ToArray();
                    AudioDataReceived?.Invoke(audioData);
                    bufferedWaveProvider.AddSamples(audioData, 0, audioData.Length);
                }
            }
        }

        private void InitializeAudio()
        {
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));
            waveOut = new WaveOutEvent();
            waveOut.Init(bufferedWaveProvider);
            waveOut.Play();
        }

    }
}
