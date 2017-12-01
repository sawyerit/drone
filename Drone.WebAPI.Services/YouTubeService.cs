using System;
using Drone.Entities.YouTube;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
	public class YouTubeService : BaseService, IYouTubeService
	{
		public Channel Create(Channel value)
		{
			try
			{
				_queueManager.AddToQueue(Utility.SerializeToXMLString<Channel>(value), "YouTube Channel " + value.Name);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "YouTubeService.Create", "Domainid: " + value.Name);
			}

			return value;
		}

		public Channel GetChannel(string user)
		{
			Channel userChannel = new Channel { Name = user, TotalSubscribers = 100, TotalVideoViews = 100 };
			return userChannel;
		}

		public ChannelVideo Get(string id)
		{
			return new ChannelVideo { ChannelVideoID = id, Likes = 10, Dislikes = 100, Title = "dislike video", ViewCount = 125 };
		}
	}
}