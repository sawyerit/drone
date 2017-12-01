using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Refrence RFC 2535
    /// </summary>
    class KeyRecord : IRecordData
    {
         public KeyRecord(DataBuffer buffer, int length)
        {
            flags = buffer.ReadShortInt();
            protocol = buffer.ReadByte();
            algorithm = buffer.ReadByte();
            publicKey = buffer.ReadBytes(length - 4);
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Flags:{0} Protocol:{1} Algorithm:{2} Public Key:{3}", flags, protocol, algorithm, publicKey);
        }


        private short flags;
        /// <summary>
        /// Return flags of record
        /// </summary>
        public short Flags          {            get { return flags; }          }
        private byte protocol;
        /// <summary>
        /// Return protocol of record
        /// </summary>
        public byte Protocol        {            get { return protocol; }       }
        private byte algorithm;
        /// <summary>
        /// return Algorithm of record
        /// </summary>
        public byte Algorithm       {            get { return algorithm; }      }
        private byte[] publicKey;
        /// <summary>
        /// return Public Key of record
        /// </summary>
        public byte[] PublicKey     {            get { return publicKey; }      }
    }
}
