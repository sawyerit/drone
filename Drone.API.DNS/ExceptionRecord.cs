using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class ExceptionRecord : TextOnly
    {
        /// <summary>
        /// Create an Excpetion record to encapsulate an exception created during the parsing of 
        /// a Data Record
        /// </summary>
        /// <param name="msg"></param>
        public ExceptionRecord(string msg)
        {
            base.Strings.Add(msg);
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
    