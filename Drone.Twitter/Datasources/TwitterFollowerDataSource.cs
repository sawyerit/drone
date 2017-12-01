using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Entities.WebAPI;
using Drone.Shared;
using System.Linq;

namespace Drone.Twitter.Datasources
{
	public class TwitterFollowerDataSource : BaseDatasource<TwitterComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;

			if (!Object.Equals(twitterDataComponent.TwitterUserList, null))
				SaveFollowerData(twitterDataComponent.TwitterUserList);
		}

		private void SaveFollowerData(List<User> list)
		{
			try
			{
                string folderName = "Twitter Followers " + DateTime.Now.ToString("MMM yyyy");
                string followerFile = String.Format("{1}\\GoDaddy_TwitterFollowers_{0:M_d_yyyy}.csv", DateTime.Today, folderName);

                Directory.CreateDirectory(Drone.Shared.Utility.ComponentBaseFolder + "\\Logs\\" + folderName);                                       

				Drone.Shared.Utility.WriteToLogFile(followerFile, "Screen Name, Follower Count", false);
				Drone.Shared.Utility.WriteToLogFile(followerFile, list.Select(x => x.screen_name + "," + x.followers_count).ToArray());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "TwitterFollowerDataSource.SaveFollowerData");
			}
		}
	}
}
