using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 1183
    /// </summary>
    class AfsdbRecord : IRecordData
    {
        /// <summary>
        /// Create data record for AFSDB type from data buffer
        /// </summary>
        /// <param name="buffer"></param>
         public AfsdbRecord(DataBuffer buffer)
        {
            subType = buffer.ReadShortInt();
            domain = buffer.ReadDomainName();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("SubType:{0} Domain:{1}", subType, domain);
        }

        private short subType;
        /// <summary>
        /// Return record subtype
        /// </summary>
        public short SubType        {            get { return subType; }        }
        private string domain;
        /// <summary>
        /// return domain name of record
        /// </summary>
        public string Domain        {            get { return domain; }        }
    }
}
