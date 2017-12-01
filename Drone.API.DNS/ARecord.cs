using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Drone.API.DNS
{
    public class ARecord : IRecordData
    {
        /// <summary>
        /// Create A Record from Data Buffer
        /// </summary>
        /// <param name="buffer"></param>
        public ARecord(DataBuffer buffer)
        {
            Byte[] ipaddress = buffer.ReadBytes(4);
            ipAddress = new IPAddress(ipaddress);
        }
        IPAddress ipAddress;
        /// <summary>
        /// Return IP Address of record
        /// </summary>
        public IPAddress IpAddress { get { return ipAddress; } }
    }
}
