using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.Portfolio;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class PortfolioController : BaseController
	{
		private readonly IPortfolioService _portfolioService;

		public PortfolioController(IPortfolioService portfolioService)
		{
			_portfolioService = portfolioService;
		}

		// GET api/portfolio
		//http://localhost:63222/api/portfolio
		public List<PortfolioDataType> Get()
		{
			return _portfolioService.GetAll();
		}

		// GET api/portfolio/shopperid or domainname
		//http://localhost:63222/api/portfolio/5 [FIVEKIDSCATERING.com]
		public List<PortfolioFullDataType> Get(string id)
		{
			return _portfolioService.Get(id); 
		}

		//get paged amount
		//http://localhost:63222/api/portfolio?page=#
		public List<PortfolioDataType> GetPaged(int page)
		{
			int count = 100;
			return _portfolioService.GetPaged(page, count);
		}

		// POST api/portfolio
		public HttpResponseMessage Post([FromBody]PortfolioDataType value)
		{
			HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError);
			if (!Object.Equals(null, value))
			{
				try
				{
					PortfolioDataType cr = _portfolioService.Create(value);
					response = Request.CreateResponse(HttpStatusCode.Created);
					response.Headers.Location = new Uri(Url.Link("DefaultApi", new { shopperid = cr.ShopperID }));
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "PortfolioController.Post(msdatatype)", "Error creating portfolio type from posted value");										
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