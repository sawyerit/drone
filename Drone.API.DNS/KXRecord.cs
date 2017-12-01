using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 2230
    /// </summary>
    class KXRecord : PrefAndDomain
    {
        public KXRecord(DataBuffer buffer) : base(buffer) { }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
