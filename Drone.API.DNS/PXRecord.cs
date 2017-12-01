using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Implementation Referecne RFC 2163
    /// </summary>
    class PXRecord : PrefAndDomain
    {
         public PXRecord(DataBuffer buffer) : base(buffer)
        {
            x400Domain = buffer.ReadDomainName();
        }

        private string x400Domain;
        public string X400Domain
        {
            get { return x400Domain; }
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
