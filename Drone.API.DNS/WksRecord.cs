using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Drone.API.DNS
{
    class WksRecord : IRecordData
    {
        /// <summary>
        /// WKS Record Type Consructor
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
         public WksRecord(DataBuffer buffer, int length)
        {
             ipAddress = buffer.ReadIPAddress();
             protocol = buffer.ReadByte();
             services = new Byte[length - 5];
             for(int i = 0; i < (length - 5); i++)
                 services[i] = buffer.ReadByte();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("IP Address:{0} Protocol:{1} Services:{2}", ipAddress, protocol, services);
        }

        IPAddress ipAddress;
        /// <summary>
        /// IP Address of record
        /// </summary>
        public IPAddress IpAddress      {   get { return ipAddress; }   }
        byte protocol;
        /// <summary>
        /// return Protocol of record
        /// </summary>
        public byte Protocol            {   get { return protocol; }    }
        byte[] services;
        /// <summary>
        /// return Services of record
        /// </summary>
        public byte[] Services          {   get { return services; }    }
    }
}
