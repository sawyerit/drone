using System.Collections.Generic;
using System.Linq;
using Google.GData.YouTube;
using Drone.Shared;
using System;

namespace Drone.Entities.YouTube
{
	public class Channel
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public List<ChannelVideo> Feed { get; set; }

		public int TotalSubscribers { get; set; }

		public int TotalVideoViews { get; set; }

		public int TotalComments
		{
			get
			{
				if (!Object.Equals(Feed, null))
					return Feed.Sum(v => v.CommentCount);
				else
					return 0;
			}
		}

		public int TotalLikes
		{
			get
			{
				if (!Object.Equals(Feed, null))
					return Feed.Sum(v => v.Likes);
				else
					return 0;
			}
		}

		public int TotalDislikes
		{
			get
			{
				if (!Object.Equals(Feed, null))
					return Feed.Sum(v => v.Dislikes);
				else
					return 0;
			}
		}

		public Channel(ProfileEntry profile, string _user, int _userID)
		{
			Name = _user;
			ID = _userID;

			if (profile.Statistics.Attributes.ContainsKey("subscriberCount"))
				TotalSubscribers = profile.Statistics.Attributes["subscriberCount"].ToString().ConvertStringToInt(0);
			else
				TotalSubscribers = 0;

			if (profile.Statistics.Attributes.ContainsKey("totalUploadViews"))
				TotalVideoViews = profile.Statistics.Attributes["totalUploadViews"].ToString().ConvertStringToInt(0);
			else
				TotalVideoViews = 0;

			Feed = new List<ChannelVideo>();
		}

		public Channel()
		{
			Feed = new List<ChannelVideo>();
		}
	}
}
