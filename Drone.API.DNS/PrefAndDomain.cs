using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class PrefAndDomain : IRecordData
    {
        /// <summary>
        /// Common class for all record types that contain a preference and a domain name
        /// </summary>
        /// <param name="buffer"></param>
        public PrefAndDomain(DataBuffer buffer)
        {
            preference = buffer.ReadBEShortInt();
            domain = buffer.ReadDomainName();
        }

        private int preference;
        /// <summary>
        /// return preference
        /// </summary>
        public int Preference
        {
            get { return preference; }
        }
        private string domain;
        /// <summary>
        /// return domain name of record
        /// </summary>
        public string Domain
        {
            get { return domain; }
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Preference:{0} Domain:{1}", preference, domain);
        }
    }
    
}
