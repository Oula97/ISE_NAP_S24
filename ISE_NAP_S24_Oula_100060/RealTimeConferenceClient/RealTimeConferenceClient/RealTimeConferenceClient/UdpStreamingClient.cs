using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RealTimeConferenceClient
{
    public class UdpStreamingClient
    {
        private UdpClient _udpClient;

        public UdpStreamingClient(int localPort)
        {
            _udpClient = new UdpClient();
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, localPort));
        }


        public void StartReceiving()
        {
            Console.WriteLine("UDP Client started receiving...");
            _ = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        IPEndPoint remoteEP = null;
                        byte[] data = _udpClient.Receive(ref remoteEP);
                        Console.WriteLine($"Received UDP packet: {Encoding.UTF8.GetString(data)}");
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"SocketException: {ex.Message}");
                        // Optionally, add logic to attempt reconnection or handle the error
                        break;
                    }
                }
            });
        }


        public async Task SendAsync(string host, int port, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(buffer, buffer.Length, host, port);
        }
    }
}
