using System;
using System.Collections.Generic;
using System.Globalization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.Twitter.v11;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class TwitterTrendDataSource : BaseDatasource<TwitterComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;

			if (!Object.Equals(twitterDataComponent, null))
				SaveTrendData(twitterDataComponent);
		}

		private static void SaveTrendData(TwitterDataComponent twitterDataComponent)
		{
			if (!Object.Equals(twitterDataComponent.TrendRootList, null) && (twitterDataComponent.TrendRootList.Count > 0))
			{
				foreach (List<TrendRoot> trendRootList in twitterDataComponent.TrendRootList)
				{
					if (trendRootList.Count > 0)
					{
						TrendRoot trendRoot = trendRootList[0];

						foreach (Trend trend in trendRoot.trends)
						{
							try
							{
								DataFactory.ExecuteNonQuery("TwitterTrendInsert",
											new KeyValuePair<string, object>("@trendQuery", Uri.UnescapeDataString(trend.query)),
											new KeyValuePair<string, object>("@WoeID", trendRoot.locations[0].woeid),
											new KeyValuePair<string, object>("@CreateDate", DateTime.ParseExact(trendRoot.created_at, @"yyyy-MM-dd\THH:mm:ss\Z", CultureInfo.InvariantCulture).AddHours(-7)));
							}
							catch (Exception e)
							{
								if (e.Message.Contains("deadlocked"))
								{
									SaveTrendData(twitterDataComponent);
									ExceptionExtensions.LogInformation("TwitterTrendDataSource.SaveTrendData", "Deadlock encountered, trying again");
								}
								else
								{
									ExceptionExtensions.LogError(e, "TwitterTrendDataSource.SaveTrendData", string.Format("woeid: {0}", trendRoot.locations.Count > 0 ? trendRoot.locations[0].woeid.ToString() : "no woeid"));

									//if tempdb full or other critical db error, re-throw
									if (Utility.IsCriticalDBError(e)) throw;
								}
							}
						}
					}
				}
			}
		}
	}
}
