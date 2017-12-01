using System;
using System.Text;
using System.Collections.Generic;

#region Rfc info
/*
3.3.14. TXT RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   TXT-DATA                    /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

TXT-DATA        One or more <character-string>s.

TXT RRs are used to hold descriptive text.  The semantics of the text
depends on the domain where it is found.
 * 
*/
#endregion

namespace Drone.API.Dig.Dns
{
	public class RecordTXT : Record
	{
		public string TXT;

		public string ASN
		{
			get { return TXT.Split('|')[0].Trim(); }
		}		

		public RecordTXT(RecordReader rr)
		{
			TXT = rr.ReadString();
		}

		public override string ToString()
		{
			return TXT;
		}

		public string COMPANY
		{
			get { return TXT.Split('|')[4].Trim(); }
		}
	}
}
