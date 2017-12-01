using System;
using System.Collections.Generic;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.Twitter.v11;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class TwitterUserDataSource : BaseDatasource<TwitterComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;

			if (!Object.Equals(twitterDataComponent, null))
				SaveFollowerData(twitterDataComponent);
		}

		private static void SaveFollowerData(TwitterDataComponent twitterDataComponent)
		{
			if (!Object.Equals(twitterDataComponent.TwitterUserList, null))
			{
				foreach (User user in twitterDataComponent.TwitterUserList)
				{
					try
					{
						if (user.id != "0" && user.followers_count != 0)
						{
							DataFactory.ExecuteNonQuery("TwitterFollowersInsert",
																					new KeyValuePair<string, object>("@HandleID", user.id),
																					new KeyValuePair<string, object>("@Followers", user.followers_count),
																					new KeyValuePair<string, object>("@ListCount", user.listed_count),
																					new KeyValuePair<string, object>("@CreateDate", DateTime.Today.AddDays(-1)));
						}
						else
						{
							ExceptionExtensions.LogWarning(new ArgumentNullException("twitter_user")
												, "TwitterDataSource.SaveFollowerData"
												, string.Format("user id: {0} - followers: {1} - list count: {2}"
																			, user.id
																			, user.followers_count
																			, user.listed_count));
						}
					}
					catch (Exception e)
					{
						if (e.Message.Contains("deadlocked"))
						{
							SaveFollowerData(twitterDataComponent);
							ExceptionExtensions.LogInformation("TwitterDataSource.SaveFollowerData", "Deadlock encountered, trying again");
						}
						else
						{
							ExceptionExtensions.LogError(e, "TwitterDataSource.SaveFollowerData", string.Format("user: {0}", user.id));

							//if tempdb full or other critical db error, re-throw
							if (Utility.IsCriticalDBError(e)) throw;
						}
					}
				}
			}
		}
	}
}
