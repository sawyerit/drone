using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class SrvRecord : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 2782
        /// </summary>
        /// <param name="buffer"></param>
         public SrvRecord(DataBuffer buffer)
        {
            priority = buffer.ReadShortInt();
            weight = buffer.ReadShortUInt();
            port = buffer.ReadShortUInt();
            domain = buffer.ReadDomainName();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Priority:{0} Weight:{1}  Port:{2} Domain:{3}", priority, weight, port, domain);
        }

        private int priority;
        /// <summary>
        /// return Priority of record
        /// </summary>
        public int Priority
        {
            get { return priority; }
        }
        private ushort weight;
        /// <summary>
        /// return Weight of record
        /// </summary>
        public ushort Weight
        {
            get { return weight; }
        }
        private ushort port;
        /// <summary>
        /// return port of record
        /// </summary>
        public ushort Port
        {
            get { return port; }
        }
        private string domain;
        /// <summary>
        /// return domain of record
        /// </summary>
        public string Domain
        {
            get { return domain; }
        }
    }
}
