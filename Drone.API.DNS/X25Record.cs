using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 1183
    /// </summary>
    class X25Record : TextOnly
    {
        /// <summary>
        /// Create X25 Record Type
        /// </summary>
        /// <param name="buffer"></param>
        public X25Record(DataBuffer buffer) : base(buffer){}
        /// <summary>
        /// return PSDN Address
        /// </summary>
        public string PsdnAddress { get { return this.Text; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
