using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class NSRecord : DomainNameOnly
    {
        /// <summary>
        /// Implementation Reference RFC 1035
        /// </summary>
        /// <param name="buffer"></param>
        public NSRecord(DataBuffer buffer) : base(buffer){}
        /// <summary>
        /// return Namje Server Domain
        /// </summary>
        public string NSDomain        {            get { return this.Domain; }        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
