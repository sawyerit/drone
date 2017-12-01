using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class IsdnRecord : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 1183
        /// </summary>
        /// <param name="buffer"></param>
         public IsdnRecord(DataBuffer buffer)
        {
            isdnAddress = buffer.ReadCharString();
            subAddress = buffer.ReadCharString();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("ISDN Address:{0} Sub Address:{1}", isdnAddress, subAddress);
        }

        private string isdnAddress;        
        /// <summary>
        /// Isdn Address of record
        /// </summary>
        public string IsdnAddress       {            get { return isdnAddress; }       }
        private string subAddress;
        /// <summary>
        /// Sub Address of record
        /// </summary>
        public string SubAddress        {            get { return subAddress; }        }
    }
}
