using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class MRRecord : DomainNameOnly
    {
        public MRRecord(DataBuffer buffer) : base(buffer) { }
        /// <summary>
        /// returnb Forwarding Address for Domain
        /// </summary>
        public string ForwardingAddress { get { return this.Domain; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
