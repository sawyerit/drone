using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 1035
    /// </summary>
    class SoaRecord : IRecordData
    {
        public SoaRecord(DataBuffer buffer)
        {
            primaryNameServer = buffer.ReadDomainName();
            responsibleMailAddress = buffer.ReadDomainName();
            serial = buffer.ReadInt();
            refresh = buffer.ReadInt();
            retry = buffer.ReadInt();
            expire = buffer.ReadInt();
            defaultTtl = buffer.ReadInt();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Primary Name Server:{0} Responsible Name Address:{1} Serial:{2} Refresh:{3} Retry:{4} Expire:{5} Default TTL:{6}",
                primaryNameServer, responsibleMailAddress, serial, refresh, retry, expire, defaultTtl);
        }

        private string primaryNameServer;
        /// <summary>
        /// Primary Name  Server of record
        /// </summary>
        public string PrimaryNameServer
        {
            get { return primaryNameServer; }
        }
        private string responsibleMailAddress;
        /// <summary>
        /// Responsible Person Mail Address 
        /// </summary>
        public string ResponsibleMailAddress
        {
            get { return responsibleMailAddress; }
        }
        private int serial;
        /// <summary>
        /// Serial of record
        /// </summary>
        public int Serial
        {
            get { return serial; }
        }
        private int refresh;
        /// <summary>
        /// Refresh of record
        /// </summary>
        public int Refresh
        {
            get { return refresh; }
        }
        private int retry;
        /// <summary>
        /// return retry of record
        /// </summary>
        public int Retry
        {
            get { return retry; }
        }
        private int expire;
        /// <summary>
        /// return expiration of record
        /// </summary>
        public int Expire
        {
            get { return expire; }
        }
        private int defaultTtl;
        /// <summary>
        /// return default ttl of record
        /// </summary>
        public int DefaultTtl
        {
            get { return defaultTtl; }
        }
    }
}
