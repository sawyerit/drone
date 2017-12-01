using System.Collections.Generic;
using Drone.Entities.Facebook;

namespace Drone.WebAPI.Interfaces
{
	public interface IFacebookService
	{
		T Create<T>(object value);
		//Demographic<Country> Create(Demographic<Country> value);
		//Demographic<Locale> Create(Demographic<Locale> value);
		//Demographic<Gender> Create(Demographic<Gender> value);
		List<Page> GetAll();
		Page Get(string id);
	}
}
