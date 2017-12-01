using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class NullRecord : TextOnly
    {
        /// <summary>
        /// Implementation Reference RFC 1035
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="?"></param>
        public NullRecord(DataBuffer buffer, int length) : base(buffer, length){}
        /// <summary>
        /// return Text of null record
        /// </summary>
        public new string Text        {  get { return base.Text; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
