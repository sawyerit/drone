
using System;
namespace Drone.Entities.MarketShare
{
	public class MarketShareDataType
	{
		public int BitMaskId { get; set; }
		public int TypeId { get; set; }
		public int DomainId { get; set; }
		public string Value { get; set; }
		public string SampleDate { get; set; }
		public Guid UniqueID { get; set; }

		public override string ToString()
		{
			return string.Format("Type:{0}, DomainID:{1}, Value:{2}, Date:{3}", TypeId, DomainId, Value, SampleDate);
		}
	}
}
