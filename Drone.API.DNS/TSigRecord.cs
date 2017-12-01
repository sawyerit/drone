using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation References RFC 2845
    /// </summary>
    class TSigRecord : IRecordData
    {
         public TSigRecord(DataBuffer buffer)
        {
            algorithm = buffer.ReadDomainName();
            timeSigned = buffer.ReadLongInt();
            fudge = buffer.ReadShortUInt();
            macSize = buffer.ReadShortUInt();
            mac = buffer.ReadBytes(macSize);
            originalId = buffer.ReadShortUInt();
            error = buffer.ReadShortUInt();
            otherLen = buffer.ReadShortUInt();
            otherData = buffer.ReadBytes(otherLen);
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Algorithm:{0} Signed Time:{1} Fudge Factor:{2} Mac:{3} Original ID:{4} Error:{5}\nOther Data:{6}",
                algorithm, timeSigned, fudge, mac, originalId, error, otherData);
        }

        private string algorithm;
        /// <summary>
        /// return Algorithm of record
        /// </summary>
        public string Algorithm
        {
            get { return algorithm; }
        }
        private long timeSigned;
        /// <summary>
        /// return signature time
        /// </summary>
        public long TimeSigned
        {
            get { return timeSigned; }
        }
        private ushort fudge;
        /// <summary>
        /// return fudge factor of record
        /// </summary>
        public ushort Fudge
        {
            get { return fudge; }
        }
        private ushort macSize;
        private byte[] mac;
        /// <summary>
        /// return MAC Address
        /// </summary>
        public byte[] Mac
        {
            get { return mac; }
        }
        private ushort originalId;
        /// <summary>
        /// return Original ID of record
        /// </summary>
        public ushort OriginalId
        {
            get { return originalId; }
        }
        private ushort error;
        /// <summary>
        /// return error of record
        /// </summary>
        public ushort Error
        {
            get { return error; }
        }
        private ushort otherLen;
        private byte[] otherData;
        /// <summary>
        /// rfeturn Other Data of record
        /// </summary>
        public byte[] OtherData
        {
            get { return otherData; }
        }
    }
}
