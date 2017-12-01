using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.YouTube;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class YouTubeController : BaseController
	{
		private readonly IYouTubeService _ytService;

		public YouTubeController(IYouTubeService service)
		{
			_ytService = service;
		}

		// GET api/youtube/id
		public ChannelVideo Get(string id)
		{
			return _ytService.Get(id);
		}

		// GET api/youtube/channel/username
		[ActionName("Channel")]
		public Channel GetChannel(string user)
		{
			return _ytService.GetChannel(user);
		}

		// POST api/youtube
		public HttpResponseMessage Post([FromBody]Channel value)
		{
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Channel cr = _ytService.Create(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				response.Headers.Location = new Uri(Url.Link("YouTubeActionApi", new { user = cr.Name, controller = "youtube", action = "channel" }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}
	}
}