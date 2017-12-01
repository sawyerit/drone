using System.Collections.Generic;
using Drone.Entities.WebAPI;

namespace Drone.WebAPI.Interfaces
{
	public interface IDomainsService
	{
		List<Domain> Get(int count);
		List<Domain> Get(int count, int mask);
		Domain Create(Domain domain);
		Domain LookupDomain(string domain);

        List<Domain> GetFromMongo(int count, string sourceCollection);
    }
}
