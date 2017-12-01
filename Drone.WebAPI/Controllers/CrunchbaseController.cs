using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.Crunchbase;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class CrunchbaseController : BaseController
	{
		private readonly ICrunchbaseService _crunchService;

		public CrunchbaseController(ICrunchbaseService crunchService)
		{
			_crunchService = crunchService;
		}

		// GET api/crunchbase
		public List<Company> Get()
		{
			return _crunchService.GetAll();
		}

		// GET api/crunchbase/companyName
		public CompanyRoot Get(string id)
		{
			CompanyRoot cr = _crunchService.Get(id);

			if (Object.Equals(null, cr))
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
								{
									Content = new StringContent(string.Format("No company with company name = {0}", id)),
									ReasonPhrase = "Company Not Found"
								};
				throw new HttpResponseException(resp);
			}
			return cr;
		}

		// POST api/crunchbase
		public HttpResponseMessage Post([FromBody]CompanyRoot value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				CompanyRoot cr = _crunchService.Create(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				response.Headers.Location = new Uri(Url.Link("DefaultApi", new { permalink = cr.permalink }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}


		//// PUT api/values/5
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE api/values/5
		//public void Delete(int id)
		//{
		//}
	}
}