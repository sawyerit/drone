using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class TKeyRecord : IRecordData
    {
        /// <summary>
        /// Implementation References RFC 2930
        /// </summary>
        /// <param name="buffer"></param>
         public TKeyRecord(DataBuffer buffer)
        {
            algorithm = buffer.ReadDomainName();
            inception = buffer.ReadUInt();
            expiration = buffer.ReadUInt();
            mode = buffer.ReadShortUInt();
            error = buffer.ReadShortUInt();
            keySize = buffer.ReadShortUInt();
            keyData = buffer.ReadBytes(keySize);
            otherSize = buffer.ReadShortUInt();
            otherData = buffer.ReadBytes(otherSize);
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Algorithm:{0} Inception:{1} Expiration:{2} Mode:{3} Error:{4} \nKey Data:{5} \nOther Data:{6} ",
                algorithm, inception, expiration, mode, error, keyData, otherData);
        }


        private string algorithm;
        /// <summary>
        /// return algorithm of record
        /// </summary>
        public string Algorithm
        {
            get { return algorithm; }
        }
        private uint inception;
        /// <summary>
        /// return inception time of record
        /// </summary>
        public uint Inception
        {
            get { return inception; }
        }
        private uint expiration;
        /// <summary>
        /// return expiration time of record
        /// </summary>
        public uint Expiration
        {
            get { return expiration; }
        }
        private ushort mode;
        /// <summary>
        /// return mode of record
        /// </summary>
        public ushort Mode
        {
            get { return mode; }
        }
        private ushort error;
        /// <summary>
        /// rfeturn error of record
        /// </summary>
        public ushort Error
        {
            get { return error; }
        }
        private ushort keySize;            
        private byte[] keyData;
        /// <summary>
        /// return Key Data of record
        /// </summary>
        public byte[] KeyData
        {
            get { return keyData; }
        }
        private ushort otherSize;
        private byte[] otherData;
        /// <summary>
        /// return Other Data of record
        /// </summary>
        public byte[] OtherData
        {
            get { return otherData; }
        }
    }
}
