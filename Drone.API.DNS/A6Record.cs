using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Drone.API.DNS
{
    class A6Record : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 3363
        /// </summary>
        /// <param name="buffer"></param>
         public A6Record(DataBuffer buffer)
        {
             prefixLength = buffer.ReadByte();
             if(prefixLength == 0) //Only Address Present
             {                 
                 ipAddress = buffer.ReadIPv6Address();
             }
             else if (prefixLength == 128) //Only Domain Name Present
             {
                 domain = buffer.ReadDomainName();
             }
             else //Address and Domain Name Present
             {
                 ipAddress = buffer.ReadIPv6Address();
                 domain = buffer.ReadDomainName();
             }
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Prefix Length:{0} IP Address:{1} Domain:{2}", prefixLength, ipAddress, domain);
        }

        private int prefixLength = -1;
        /// <summary>
        /// Return prefix length
        /// </summary>
        public int PrefixLength         {   get { return prefixLength; }    }
        private IPAddress ipAddress;
        /// <summary>
        /// Return IP Address of the data record
        /// </summary>
        public IPAddress IpAddress      {   get { return ipAddress; }       }
        private string domain;
        /// <summary>
        /// Return Domain name of data record
        /// </summary>
        public string Domain            {   get { return domain; }          }
    }
}
