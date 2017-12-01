using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Answer type derived from DNSEntry
    /// </summary>
    public class Answer : DnsEntry
    {
        public Answer(DataBuffer buffer) : base(buffer)     {}
    }
}
