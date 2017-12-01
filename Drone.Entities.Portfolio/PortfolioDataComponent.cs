using System.Data;
using Drone.Core;
using Drone.Entities.Portfolio;

namespace Drone.Entities.Portfolio
{
	public class PortfolioDataComponent : BaseDataComponent<PortfolioComponent>
	{
		public PortfolioDataType PortfolioType { get; set; }
	}
}
