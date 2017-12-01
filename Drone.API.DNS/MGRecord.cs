using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Reference RFC 1035
    /// </summary>
    class MGRecord : DomainNameOnly
    {
        public MGRecord(DataBuffer buffer) : base(buffer) { }
        /// <summary>
        /// return Mail Group Domain
        /// </summary>
        public string MailGroupDomain { get { return this.Domain; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
