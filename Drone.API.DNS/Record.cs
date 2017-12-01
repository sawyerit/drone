using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class Record : DnsEntry
    {
        /// <summary>
        /// Create a record for additional records in Dns Answer
        /// </summary>
        /// <param name="buffer"></param>
        public Record(DataBuffer buffer) : base(buffer) { }
    }
}
