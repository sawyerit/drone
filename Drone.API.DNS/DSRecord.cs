using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class DSRecord : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 3658
        /// </summary>
        /// <param name="buffer"></param>
         public DSRecord(DataBuffer buffer, int length)
        {
             key = buffer.ReadShortInt();
             algorithm = buffer.ReadByte();
             digestType = buffer.ReadByte();
             digest = buffer.ReadBytes(length - 4);
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Key:{0} Algorithm:{1} DigestType:{2} Digest:{3}", key, algorithm, digestType, digest);
        }

        short key;
        /// <summary>
        /// Return Record Key
        /// </summary>
        public short Key
        {
            get { return key; }
        }
        byte algorithm;
        /// <summary>
        /// Return Record Algorithm
        /// </summary>
        public byte Algorithm
        {
            get { return algorithm; }
        }
        byte digestType;
        /// <summary>
        /// return record Digest Type
        /// </summary>
        public byte DigestType
        {
            get { return digestType; }
        }
        byte[] digest;
        /// <summary>
        /// Retuirn Record Digest
        /// </summary>
        public byte[] Digest
        {
            get { return digest; }
        }
    }
}
