using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RealTimeConferenceClient
{
    public class Room
    {
        public string Name { get; set; }
        public int MemberCount { get; set; } = 0;
        public int MaxMembers { get; set; } = 10; // Default maximum
        public List<WebSocket> WebSockets { get; set; } = new List<WebSocket>();

        public override string ToString()
        {
            return $"{Name} ({MemberCount}/{MaxMembers} members)";
        }

        public bool AddMember(WebSocket webSocket)
        {
            if (MemberCount >= MaxMembers)
                return false;

            WebSockets.Add(webSocket);
            MemberCount++;
            return true;
        }

        public bool RemoveMember(WebSocket webSocket)
        {
            if (WebSockets.Remove(webSocket))
            {
                MemberCount--;
                return true;
            }
            return false;
        }
    }
}
