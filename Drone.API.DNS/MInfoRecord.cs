using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class MInfoRecord : IRecordData
    {
         public MInfoRecord(DataBuffer buffer)
        {
            responsibleMailbox = buffer.ReadDomainName();
            errorMailbox = buffer.ReadDomainName();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Responsible Mailbox:{0} Error Mailbox:{1}", responsibleMailbox, errorMailbox);
        }

        private string responsibleMailbox;
        /// <summary>
        /// rfeturn Responsible Person Mail Box
        /// </summary>
        public string ResponsibleMailbox    {   get { return responsibleMailbox; }  }
        private string errorMailbox;
        /// <summary>
        /// return Error Reporting Mail box
        /// </summary>
        public string ErrorMailbox          {   get { return errorMailbox; }        }
    }
}
