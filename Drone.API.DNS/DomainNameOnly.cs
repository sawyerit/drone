using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class DomainNameOnly : IRecordData
    {
        /// <summary>
        /// Base class for all Record Types that have RDATA consisting of a single domain name
        /// </summary>
        /// <param name="buffer"></param>
        public DomainNameOnly(DataBuffer buffer)
        {
            domain = buffer.ReadDomainName();
        }
        private string domain;
        protected string Domain        {            get { return domain; }          }
        /// <summary>
        /// Return string representing record
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Domain: " + Domain;
        }
    }
}
