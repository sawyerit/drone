using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class Question
    {
        /// <summary>
        /// Create a Question record for Dns Answer
        /// </summary>
        /// <param name="buffer"></param>
        public Question(DataBuffer buffer)
        {
            domain = buffer.ReadDomainName();
            recType = (RecordType)buffer.ReadBEShortInt();
            classType = buffer.ReadBEShortInt();

        }

        string domain;
        /// <summary>
        /// Return domain name of question
        /// </summary>
        public string Domain
        {
            get { return domain; }
        }
        RecordType recType;
        /// <summary>
        /// return Record Type of Question
        /// </summary>
        public RecordType RecType
        {
            get { return recType; }
        }
        int classType;
        /// <summary>
        /// return Class Type of question
        /// </summary>
        public int ClassType
        {
            get { return classType; }
        }
    }
}
