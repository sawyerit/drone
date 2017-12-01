using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Drone.API.Twitter;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.Twitter.Datasources;

namespace Drone.Twitter.Components
{
	[Export(typeof(IDroneComponent))]
	public class TwitterUser : BaseComponent<TwitterComponent>
	{
		#region public properties

		//public List<User> TwitterUserList { get; set; }
		internal TwitterDataComponent _dataComponent;

		#endregion

		#region constructors

		[ImportingConstructor]
		public TwitterUser()
			: base()
		{
			DroneDataSource = new TwitterUserDataSource();
			DroneDataComponent = new TwitterDataComponent();
		}

		public TwitterUser(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
			DroneDataComponent = new TwitterDataComponent();
		}

		#endregion

		/// <summary>
		/// Main method that gathers data
		/// </summary>
		/// <param name="context">iDroneContext</param>
		public override void GetData(object context)
		{
			try
			{
				BaseContext cont = context as BaseContext;

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);

					DateTime tempNextRun = cont.NextRun;
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (DateTime.Compare(tempNextRun, DateTime.MinValue) != 0 && XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						WriteToUsageLogFile("TwitterUser.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetTwitterUsers Competitors");

						//do work
						TwitterDataComponent _dataComponent = DroneDataComponent as TwitterDataComponent;
						_dataComponent.TwitterUserList = GetTwitterUsers();

						DroneDataSource.Process(_dataComponent);

						WriteToUsageLogFile("TwitterUser.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetTwitterUsers Competitors");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.Twitter.Components.TwitterUser.GetData()");
			}
		}

		#region private methods

		/// <summary>
		/// Twitter user info, used to get total follower count
		/// </summary>
		internal List<User> GetTwitterUsers()
		{
			List<User> _twitterUserList = new List<User>();
			TwitterUserDataSource data = new TwitterUserDataSource();
			UserManager um = new UserManager();

			foreach (Competitor comp in data.GetCompetitorAccounts())
			{
				if (comp.TwitterID != 0)
					_twitterUserList.Add(um.GetTwitterUserInfo(comp.TwitterID, Utility.GetOAuthToken(Xml)));
			}

			return _twitterUserList;
		}


		#endregion

		#region WebService methods

		//public static User GetTwitterUserFollowerCount(long twitterID)
		//{
		//	return new UserManager().GetTwitterUserInfo(twitterID);
		//}

		#endregion
	}
}
