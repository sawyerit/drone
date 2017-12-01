using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class MBRecord : DomainNameOnly
    {
        /// <summary>
        /// Implementation Reference RFC 1035
        /// </summary>
        /// <param name="buffer"></param>
        public MBRecord(DataBuffer buffer) : base(buffer){}
        /// <summary>
        /// return Administators Mail Box
        /// </summary>
        public string AdminMailboxDomain { get { return this.Domain; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
