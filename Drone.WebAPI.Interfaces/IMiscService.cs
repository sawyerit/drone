using System.Collections.Generic;
using Drone.Entities.WebAPI;
using System.Data;

namespace Drone.WebAPI.Interfaces
{
	public interface IMiscService
	{
        List<Dictionary<string, object>> CompareQueries(string q1, string q2, string username);
	}
}
