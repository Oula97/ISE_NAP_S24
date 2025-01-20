using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RealTimeConferenceServer
{
    internal class UdpStreamingServer
    {
        private UdpClient udpServer;

        public UdpStreamingServer(int port)
        {
            udpServer = new UdpClient(port);
        }

        public void Start()
        {
            Console.WriteLine("UDP Streaming Server started.");
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    // استلام البيانات
                    byte[] receivedData = udpServer.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Data received from {remoteEndPoint.Address}:{remoteEndPoint.Port}");

                    // طباعة البيانات المستلمة
                    Console.WriteLine($"Received: {Encoding.UTF8.GetString(receivedData)}");

                    // إعداد استجابة
                    byte[] responseData = Encoding.UTF8.GetBytes("Hello from server");

                    // إرسال الاستجابة إلى العميل
                    udpServer.Send(responseData, responseData.Length, remoteEndPoint);
                    Console.WriteLine("Response sent back to the client.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
