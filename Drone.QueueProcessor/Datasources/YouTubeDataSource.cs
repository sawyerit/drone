using System;
using System.Collections.Generic;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.YouTube;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class YouTubeDataSource : BaseDatasource<YouTubeComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			YouTubeDataComponent ytdc = component as YouTubeDataComponent;
			if (!Object.Equals(ytdc, null))
			{
				SaveChannelInfo(ytdc);
			}

		}

		private static void SaveChannelInfo(YouTubeDataComponent ytdc)
		{
			ChannelVideo curVideo = null;

			if (!Object.Equals(ytdc.YouTubeChannel, null))
			{
				try
				{
					//channel & competitors
					if (ytdc.YouTubeChannel.ID != 0) //0 is squatter videos
					{
						DataFactory.ExecuteNonQuery("YouTubeCompetitorChannelInsert",
															new KeyValuePair<string, object>("@CompetitorID", ytdc.YouTubeChannel.ID),
															new KeyValuePair<string, object>("@Views", ytdc.YouTubeChannel.TotalVideoViews),
															new KeyValuePair<string, object>("@Subscribers", ytdc.YouTubeChannel.TotalSubscribers),
															new KeyValuePair<string, object>("@Likes", ytdc.YouTubeChannel.TotalLikes),
															new KeyValuePair<string, object>("@Dislikes", ytdc.YouTubeChannel.TotalDislikes),
															new KeyValuePair<string, object>("@Comments", ytdc.YouTubeChannel.TotalComments),
															new KeyValuePair<string, object>("@ReportDate", DateTime.Today.AddDays(-1)));
					}

					//if its GD and we have videos.  To store video detail info from competitors too. remove this check for GD
					if (ytdc.YouTubeChannel.ID <= 1 && !Object.Equals(ytdc.YouTubeChannel.Feed, null)) //1 is GD, 0 is Squatters
					{
						foreach (var video in ytdc.YouTubeChannel.Feed)
						{
							curVideo = video;
							DataFactory.ExecuteNonQuery("YouTubeVideoInsert",
																					new KeyValuePair<string, object>("@CommercialID", video.ChannelVideoID),
																					new KeyValuePair<string, object>("@CommercialName", video.Title),
																					new KeyValuePair<string, object>("@isGoDaddyChannel", ytdc.YouTubeChannel.ID == 1 ? 1 : 0));

							DataFactory.ExecuteNonQuery("YouTubeVideoDetailInsert",
																					new KeyValuePair<string, object>("@CommercialID", video.ChannelVideoID),
																					new KeyValuePair<string, object>("@Views", video.ViewCount),
																					new KeyValuePair<string, object>("@Comments", video.CommentCount),
																					new KeyValuePair<string, object>("@Likes", video.Likes),
																					new KeyValuePair<string, object>("@Dislikes", video.Dislikes),
																					new KeyValuePair<string, object>("@DateUploaded", video.UploadedDate),
																					new KeyValuePair<string, object>("@ReportDate", DateTime.Today.AddDays(-1)));
						}
					}
				}
				catch (Exception e)
				{
					if (e.Message.Contains("deadlocked"))
					{
						SaveChannelInfo(ytdc);
						ExceptionExtensions.LogInformation("YouTubeDataSource.SaveChannelInfo", "Deadlock encountered, trying again");
					}
					else
					{
						ExceptionExtensions.LogError(e, "YouTubeDataSource.SaveChannelInfo", "Video ID: " + curVideo.ChannelVideoID);

						//if tempdb full or other critical db error, re-throw
						if (Utility.IsCriticalDBError(e)) throw;
					}
				}

			}
		}
	}
}
