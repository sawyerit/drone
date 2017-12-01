using System.Data;
using Drone.Core;
using Drone.Entities.MarketShare;

namespace Drone.Entities.MarketShare
{
	public class MarketShareDataComponent : BaseDataComponent<MarketShareComponent>
	{
		public MarketShareDataType MarketShareType { get; set; }
	}
}
