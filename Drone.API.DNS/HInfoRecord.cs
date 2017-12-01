using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class HInfoRecord : TextOnly
    {
        /// <summary>
        /// Implementation Reference RFC 1035
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public HInfoRecord(DataBuffer buffer, int length) : base(buffer, length){}    
        /// <summary>
        /// return Record CPU Type
        /// </summary>
        public string Cpu      
        {            
            get 
            {
                if (this.Count > 0)
                    return this.Strings[0];
                else
                    return "Unknown";
            }    
        }
        /// <summary>
        /// Return Record Operating System
        /// </summary>
        public string Os 
        { 
            get 
            {
                if (this.Count > 1)
                    return this.Strings[1];
                else
                    return "Unknown";
            } 
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
