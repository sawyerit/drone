using System.Collections.Generic;
using Drone.Entities.Crunchbase;

namespace Drone.WebAPI.Interfaces
{
	public interface ICrunchbaseService
	{
		CompanyRoot Create(CompanyRoot value);
		List<Company> GetPaged(int page);
		List<Company> GetAll();
		CompanyRoot Get(string id);
	}
}
