using System.Data;

namespace Drone.Entities.Portfolio
{
	public class PortfolioBulkData
	{
		public int LastCount { get; set; }
		public DataTable BulkTable { get; set; }
	}
}
