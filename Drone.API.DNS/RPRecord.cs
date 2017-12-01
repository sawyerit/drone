using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class RPRecord : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 1183
        /// </summary>
        /// <param name="buffer"></param>
        public RPRecord(DataBuffer buffer)
        {
            responsibleMailbox = buffer.ReadDomainName();
            textDomain = buffer.ReadDomainName();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Responsible Mailbox:{0} Text Domain:{1}", responsibleMailbox, textDomain);
        }

        private string responsibleMailbox;
        /// <summary>
        /// return Responsible Person Mailbox
        /// </summary>
        public string ResponsibleMailbox        {            get { return responsibleMailbox; }         }
        private string textDomain;
        /// <summary>
        /// return Domain for Test responses from Responsible Person
        /// </summary>
        public string TextDomain                {            get { return textDomain; }                 }
    }
}
