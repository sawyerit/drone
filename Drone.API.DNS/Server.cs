using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class Server : DnsEntry
    {
        /// <summary>
        /// Ceate A server Record for server section of Dns Response
        /// </summary>
        /// <param name="buffer"></param>
        public Server(DataBuffer buffer) : base(buffer)     {}
    }
}
