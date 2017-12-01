using System.Collections.Generic;
using Drone.Entities.WebAPI;

namespace Drone.WebAPI.Interfaces
{
	public interface ICommonService
	{
		List<Competitor> GetCompetitors();
		Competitor GetCompetitor(int id);
        List<string> PeekQueue(int count);
    }
}
