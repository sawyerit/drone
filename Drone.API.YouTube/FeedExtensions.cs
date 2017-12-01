using System;
using System.Collections.Generic;
using System.Linq;
using Google.YouTube;
using Drone.Entities.YouTube;
using Drone.Shared;

namespace Drone.API.YouTube
{
	public static class FeedExtensions
	{
		public static List<ChannelVideo> MostRecent(this List<ChannelVideo> feed, int count = 5)
		{
			return feed.Take(count).ToList();
		}

		public static List<ChannelVideo> MostPopular(this List<ChannelVideo> feed, int count = 5)
		{
			return feed.OrderByDescending(v => v.ViewCount).Take(count).ToList();
		}
	}
}
