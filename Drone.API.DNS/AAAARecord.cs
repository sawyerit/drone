using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Drone.API.DNS
{
    class AAAARecord : IRecordData
    { 
        /// <summary>
        /// Implementation Reference RFC 3596
        /// </summary>
        /// <param name="buffer"></param>
         public AAAARecord(DataBuffer buffer)   {   ipAddress = buffer.ReadIPv6Address();   }

        IPAddress ipAddress;
        /// <summary>
        /// Return IP Address of the record
        /// </summary>
        public IPAddress IpAddress
        {   
            get {
                return ipAddress;
            }   }
        /// <summary>
        /// Converts this data record to a string 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        { 
            return "IP Address: " + ipAddress.ToString();
        }
     }
 }

