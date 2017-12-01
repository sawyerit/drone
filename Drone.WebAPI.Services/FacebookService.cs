using System;
using System.Collections.Generic;
using Drone.Entities.Facebook;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
	public class FacebookService : BaseService, IFacebookService
	{
		public List<Page> GetAll()
		{
			Page p = new Page { Id = "godaddy", Likes = 150 };
			Page p1 = new Page { Id = "1and1", Likes = 50 };
			List<Page> pList = new List<Page>();
			pList.Add(p); pList.Add(p1);
			return pList;
		}

		public Page Get(string id)
		{
			return new Page { Id = id };
		}

		public T Create<T>(object value)
		{
			try
			{
				_queueManager.AddToQueue(Utility.SerializeToXMLString<T>(value), typeof(T).Name);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "FacebookService.Create", "Facebook " + typeof(T).Name);
			}

			return (T)value;
		}
	}
}