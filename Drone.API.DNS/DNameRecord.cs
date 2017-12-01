using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class DNameRecord : DomainNameOnly
    {
        /// <summary>
        /// Implementation Reference RFC 2672
        /// </summary>
        /// <param name="buffer"></param>
        public DNameRecord(DataBuffer buffer) : base(buffer){}
        /// <summary>
        /// return domain name of the record
        /// </summary>
        public string DomainName { get { return this.Domain; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
