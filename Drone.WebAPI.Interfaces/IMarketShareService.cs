using System.Collections.Generic;
using Drone.Entities.MarketShare;
using Drone.Entities.WebAPI;

namespace Drone.WebAPI.Interfaces
{
	public interface IMarketShareService
	{
		MarketShareDataType Create(MarketShareDataType value);
		List<MarketShareDataType> GetAll();
		List<MarketShareDataType> GetPaged(int page, int count);
		List<MarketShareDataType> Get(int id);
	}
}
