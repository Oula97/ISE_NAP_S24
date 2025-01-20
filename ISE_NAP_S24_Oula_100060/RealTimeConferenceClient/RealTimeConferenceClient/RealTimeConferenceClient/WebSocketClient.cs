using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeConferenceClient
{
    internal class WebSocketClient
    {
        private ClientWebSocket _client;

        public WebSocketClient()
        {
            _client = new ClientWebSocket();
        }

        public async Task ConnectAsync(string uri)
        {
            try
            {
                await _client.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine("Connected to WebSocket server.");
                _ = Task.Run(() => ListenAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Error: {ex.Message}");
            }
        }

        public async Task SendAsync(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ListenAsync()
        {
            byte[] buffer = new byte[1024];
            while (_client.State == WebSocketState.Open)
            {
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received from WebSocket server: {message}");
            }
        }

        private async Task HandleClientAsync(WebSocket webSocket, CancellationToken token)
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", token);
                        Console.WriteLine("WebSocket connection closed by client.");
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received: {message}");

                        // إرسال الرد إلى العميل
                        byte[] response = Encoding.UTF8.GetBytes($"Echo: {message}");
                        await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                if (webSocket.State != WebSocketState.Closed)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", token);
                }
                webSocket.Dispose();
            }
        }

    }
}
