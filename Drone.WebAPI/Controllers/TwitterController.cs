using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.Twitter.v11;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class TwitterController : BaseController
	{
		private readonly ITwitterService _tweetService;

		public TwitterController(ITwitterService twitterService)
		{
			_tweetService = twitterService;
		}

		// GET api/twitter/mentions
		//http://localhost:63222/api/twitter/mentions
		[ActionName("Mentions")]
		public List<KeywordStatus> Get()
		{
			return _tweetService.GetAllMentions(); 
		}

		//GET api/twitter/mentions/page number
		//http://localhost:63222/api/twitter/mentions/?page=5
		[ActionName("Mentions")]
		public List<KeywordStatus> GetByPage(int page)
		{
			return _tweetService.GetPaged(page, 100);
		}

		// GET api/twitter/mentions/id
		//http://localhost:63222/api/twitter/mentions/5
		[ActionName("Mentions")]
		public Status Get(int id)
		{
			return _tweetService.GetMention(id);
		}

		//get keywords
		//http://localhost:63222/api/twitter/keywords
		[ActionName("Keywords")]
		public List<Keyword> GetKeywords()
		{
			return _tweetService.GetKeywords();
		}

		// POST api/twitter/Mentions
		[ActionName("Mentions")]
		public HttpResponseMessage Post([FromBody]KeywordStatus value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				KeywordStatus cr = _tweetService.CreateMentions(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = 1 }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}

		// POST api/twitter/Users
		[ActionName("Users")]
		public HttpResponseMessage Post(List<User> value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				List<User> cr = _tweetService.CreateUsers(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				//response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = 1 }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}
	}
}