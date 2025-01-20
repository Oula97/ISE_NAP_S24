using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeConferenceClient
{
    internal class TcpChatClient
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public async Task ConnectAsync(string host, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            _stream = _client.GetStream();
            Console.WriteLine("Connected to TCP chat server.");

            _ = Task.Run(() => ListenAsync());
        }

        public async Task SendMessageAsync(string message)
        {
            if (_stream != null && _client.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(buffer, 0, buffer.Length);

            }
        }

        private async Task ListenAsync()
        {
            byte[] buffer = new byte[1024];
            while (_client.Connected)
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received from chat server: {message}");
            }
        }
    }
}
