using Drone.Shared;
using System;
using Google.GData.YouTube;
using Google.YouTube;

namespace Drone.Entities.YouTube
{
	public class ChannelVideo
	{
		public int ViewCount { get; set; }
		public int CommentCount { get; set; }
		public int Likes { get; set; }
		public int Dislikes { get; set; }
		public string Title { get; set; }
		public string ChannelVideoID { get; set; }
		public DateTime UploadedDate { get; set; }

		public ChannelVideo() { }

		public ChannelVideo(Video vid)
		{
			ViewCount = vid.ViewCount;
			CommentCount = vid.CommmentCount;
			Title = vid.Title;
			ChannelVideoID = vid.Id.Split(':')[3];
			UploadedDate = vid.YouTubeEntry.Published;

			if (!Object.Equals(vid.YouTubeEntry, null) && !Object.Equals(vid.YouTubeEntry.YtRating, null))
			{
				YtRating ytrat = vid.YouTubeEntry.YtRating;
				Likes = ytrat.NumLikes.ConvertStringToInt(0);
				Dislikes = ytrat.NumDislikes.ConvertStringToInt(0);
			}
		}
	}
}
