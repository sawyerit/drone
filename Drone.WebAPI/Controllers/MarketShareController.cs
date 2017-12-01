using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.MarketShare;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class MarketShareController : BaseController
	{
		private readonly IMarketShareService _marketService;

		public MarketShareController(IMarketShareService marketShareService)
		{
			_marketService = marketShareService;
		}

		// GET api/marketshare
		//http://localhost:63222/api/marketshare
		public List<MarketShareDataType> Get()
		{
			return _marketService.GetAll();
		}

		// GET api/marketshare/id
		//http://localhost:63222/api/marketshare/5
		public List<MarketShareDataType> Get(int id)
		{
			return _marketService.Get(id);
		}

		//get paged amount
		//http://localhost:63222/api/marketshare?page=#
		public List<MarketShareDataType> GetPaged(int page)
		{
			int count = 100;
			return _marketService.GetPaged(page, count);
		}

		// POST api/marketshare
		public HttpResponseMessage Post([FromBody]MarketShareDataType value)
		{
			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
			if (!Object.Equals(null, value))
			{
				try
				{
					MarketShareDataType cr = _marketService.Create(value);
					response = Request.CreateResponse(HttpStatusCode.Created);
					response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = cr.DomainId }));
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "MarketShareController.Post(msdatatype)", "Error creating marketshare type from posted value");										
				}
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}
	}
}