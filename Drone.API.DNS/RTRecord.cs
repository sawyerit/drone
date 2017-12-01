using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class RTRecord : PrefAndDomain
    {
        /// <summary>
        /// Implementation Reference  RFC 1183 
        /// </summary>
        /// <param name="buffer"></param>
         public RTRecord(DataBuffer buffer) : base(buffer){}
        /// <summary>
         /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
