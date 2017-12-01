using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    /// <summary>
    /// Base class for all Record types that caontain RDATA consisting of a single text string
    /// </summary>
    public class TextOnly : IRecordData
    {
        /// <summary>
        /// default Text Constructor
        /// </summary>
        public TextOnly() { text = new List<string>(); }
        /// <summary>
        /// create a text array from record bytes
        /// </summary>
        /// <param name="buffer"> buffer of bytes</param>
        public TextOnly(DataBuffer buffer) 
        {
            text = new List<string>();
            while(buffer.Next > 0)
                text.Add(buffer.ReadCharString()); 
        }
        /// <summary>
        /// create a text array from record bytes
        /// </summary>
        /// <param name="buffer"> buffer of bytes </param>
        /// <param name="length"> length of bytes to use</param>
        public TextOnly(DataBuffer buffer, int length)
        {
            int len = length;
            int pos = buffer.Position;
            text = new List<string>();
            byte next = buffer.Next;
            while (length > 0)
            {
                text.Add(buffer.ReadCharString());
                length -= next + 1;
                if (length < 0)
                {
                    buffer.Position = pos - len;  //Reset current Pointer of Buffer to end of this record
                    throw new DnsQueryException("Buffer Over Run in TextOnly Record Data Type", null);
                }
                next = buffer.Next;
            }
            if (length > 0)
            {
                buffer.Position = pos - len;  //Reset current Pointer of Buffer to end of this record
                throw new DnsQueryException("Buffer Under Run in TextOnly Record Data Type", null);
            }
        }
        private List<string> text;
        /// <summary>
        /// text of record
        /// </summary>
        protected string Text
        {
            get 
            {
                string res = String.Empty;                
                foreach (string s in text)
                    res += s + "\n"; 
                return res;
            }
        }
        /// <summary>
        /// return number of strings in text record
        /// </summary>
        protected int Count             { get { return text.Count; } }
        /// <summary>
        /// return list of strings in record
        /// </summary>
        protected List<string> Strings  { get { return text; } }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Text: " + Text;
        }
    }
}
