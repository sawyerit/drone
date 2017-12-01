using Drone.Entities.MarketShare;
using Drone.Shared;
using Drone.WebAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Drone.WebAPI.Services
{
	public class MarketShareService : BaseService, IMarketShareService
	{
		public MarketShareDataType Create(MarketShareDataType value)
		{
			try
			{
                value.Value = Regex.Replace(value.Value, "[^\x20-\x7E]", String.Empty);
				_queueManager.AddToQueue(Utility.SerializeToXMLString<MarketShareDataType>(value), "MarketShare BitMaskID " + value.BitMaskId.ToString());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "MarketShareService.Create", "Domainid: " + value.DomainId);
			}

			return value;
		}

		public List<MarketShareDataType> GetAll()
		{
			return new List<MarketShareDataType>();
		}

		public List<MarketShareDataType> GetPaged(int page, int count)
		{
			return new List<MarketShareDataType>();
		}

		public List<MarketShareDataType> Get(int id)
		{
			List<MarketShareDataType> list = new List<MarketShareDataType>();
			list.Add(new MarketShareDataType { DomainId = id, BitMaskId = 4, Value = "GoDaddy", SampleDate = DateTime.Now.ToString(), TypeId = 3 });
			return list;
		}
	}
}