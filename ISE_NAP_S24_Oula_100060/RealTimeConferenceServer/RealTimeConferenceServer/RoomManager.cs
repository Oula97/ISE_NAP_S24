using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeConferenceServer
{
    internal class RoomManager
    {
        private Dictionary<string, List<TcpClient>> rooms = new();

        public void CreateRoom(string roomName)
        {
            if (!rooms.ContainsKey(roomName))
            {
                rooms[roomName] = new List<TcpClient>();
                Console.WriteLine($"Room '{roomName}' created.");
            }
        }

        public void DeleteRoom(string roomName)
        {
            if (rooms.ContainsKey(roomName))
            {
                rooms.Remove(roomName);
                Console.WriteLine($"Room '{roomName}' deleted.");
            }
        }

        public void AddClientToRoom(string roomName, TcpClient client)
        {
            if (rooms.ContainsKey(roomName))
            {
                rooms[roomName].Add(client);
                Console.WriteLine($"Client added to room '{roomName}'.");
            }
        }
    }
}
