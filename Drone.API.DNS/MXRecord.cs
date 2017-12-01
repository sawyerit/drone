using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 
    /// </summary>
    public class MXRecord : PrefAndDomain
    {        
        public MXRecord(DataBuffer buffer) : base(buffer){}
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
