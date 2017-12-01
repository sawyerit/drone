using System.Data;

namespace Drone.Entities.MarketShare
{
	public class MarketShareBulkData
	{
		public int LastCount { get; set; }
		public DataTable BulkTable { get; set; }
	}
}
