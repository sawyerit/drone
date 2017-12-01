using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class PtrRecord : DomainNameOnly
    {
        public PtrRecord(DataBuffer buffer):base(buffer){}
        /// <summary>
        /// return domain name of record
        /// </summary>
        public string PtrDomain { get { return this.Domain; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
