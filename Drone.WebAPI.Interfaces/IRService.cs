using System.Collections.Generic;
using Drone.Entities.Portfolio;
using System.Xml.Linq;

namespace Drone.WebAPI.Interfaces
{
	public interface IRService
	{
		XElement Get(string id, string parms);

        XElement Get(string id);
    }
}
