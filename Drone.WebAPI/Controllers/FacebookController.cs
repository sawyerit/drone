using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.Facebook;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class FacebookController : BaseController
	{
		private readonly IFacebookService _faceService;

		public FacebookController(IFacebookService faceService)
		{
			_faceService = faceService;
		}

		// GET api/facebook
		public List<Entities.Facebook.Page> Get()
		{
			return _faceService.GetAll();
		}

		// GET api/facebook/facebook-id
		public Entities.Facebook.Page Get(string id)
		{
			return _faceService.Get(id);
		}

		// POST api/facebook/page
		[ActionName("Page")]
		public HttpResponseMessage Post([FromBody]Page value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Page page = _faceService.Create<Page>(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				response.Headers.Location = new Uri(Url.Link("FacebookPostApi", new { id = page.Id }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}

		// POST api/facebook/country
		[ActionName("Country")]
		public HttpResponseMessage Post([FromBody]Demographic<Country> value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Demographic<Country> country = _faceService.Create<Demographic<Country>>(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = page.Id }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}

		// POST api/facebook/locale
		[ActionName("Locale")]
		public HttpResponseMessage Post([FromBody]Demographic<Locale> value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Demographic<Locale> locale = _faceService.Create<Demographic<Locale>>(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = locale.Data. }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}

		// POST api/facebook/gender
		[ActionName("Gender")]
		public HttpResponseMessage Post([FromBody]Entities.Facebook.Demographic<Gender> value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Demographic<Gender> gender = _faceService.Create<Demographic<Gender>>(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = locale.Data. }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}
	}
}