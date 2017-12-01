using System.Collections.Generic;
using Drone.Entities.Portfolio;

namespace Drone.WebAPI.Interfaces
{
	public interface IPortfolioService
	{
		PortfolioDataType Create(PortfolioDataType value);
		List<PortfolioFullDataType> Get(string id);
		List<PortfolioDataType> GetAll();
		List<PortfolioDataType> GetPaged(int page, int count);		
	}
}
