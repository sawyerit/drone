using System;
/*
 3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */
namespace Drone.API.Dig.Dns
{
	public class RecordA : Record
	{
		public System.Net.IPAddress Address;

		public RecordA(RecordReader rr)
		{
			Address = new System.Net.IPAddress(rr.ReadBytes(4));
		}

		public override string ToString()
		{
			return Address.ToString();
		}

	}
}
