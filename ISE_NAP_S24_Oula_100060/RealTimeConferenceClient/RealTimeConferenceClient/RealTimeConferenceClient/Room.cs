using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets; // Add this using directive


namespace RealTimeConferenceClient
{
    public class Room
    {

        public string Name { get; set; }
        public int MemberCount { get; set; } = 0;
        public int MaxMembers { get; set; } = 10; // الحد الأقصى الافتراضي
        public List<WebSocket> WebSockets { get; set; } // Add this property

        public override string ToString()
        {
            return $"{Name} ({MemberCount}/{MaxMembers} members)";
        }

    }
}
