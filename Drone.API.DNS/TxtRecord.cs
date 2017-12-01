using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    public class TxtRecord : TextOnly
    {
        /// <summary>
        /// Text Record Type
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public TxtRecord(DataBuffer buffer, int length) : base(buffer, length){}
        public new string Text { get { return base.Text; } }
        public string ASN
        {
            get { return Text.Split('|')[0].Trim(); }
        }
        public string COMPANY
        {
            get { return Text.Split('|')[4].Trim(); }
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return base.ToString(); }
    }
}
