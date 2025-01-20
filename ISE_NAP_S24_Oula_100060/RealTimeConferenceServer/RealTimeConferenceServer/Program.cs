// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;

namespace RealTimeConferenceServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tcpChatServer = new TcpChatServer(5000);
            var udpStreamingServer = new UdpStreamingServer(6000);
            var webSocketServer = new WebSocketServer("http://localhost:8080/");

            // Start servers in separate tasks
            Task.Run(() => tcpChatServer.Start());
            Task.Run(() => udpStreamingServer.Start());
            await webSocketServer.StartAsync();

            Console.WriteLine("Server is running. Press Enter to stop.");
            Console.ReadLine();

            // Stop the servers if needed (add logic to stop the servers cleanly)
        }
    }
}
