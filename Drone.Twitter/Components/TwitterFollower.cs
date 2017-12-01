using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Drone.API.Twitter;
using Drone.API.Twitter.OAuth;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.Twitter.Datasources;

namespace Drone.Twitter.Components
{
	[Export(typeof(IDroneComponent))]
	public class TwitterFollower : BaseComponent<TwitterComponent>
	{
		#region public properties

		//public List<User> TwitterUserList { get; set; }
		internal TwitterDataComponent _dataComponent;

		#endregion

		#region constructors

		[ImportingConstructor]
		public TwitterFollower()
			: base()
		{
			DroneDataSource = new TwitterFollowerDataSource();
			DroneDataComponent = new TwitterDataComponent();
		}

		public TwitterFollower(IDroneDataSource datasource)
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

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						WriteToUsageLogFile("TwitterFollower.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetTwitterFollowers");

						//wait 5 min for other twitter processes to finish so we don't get rate limited
						Thread.Sleep(300000);

						//do work
						TwitterDataComponent _dataComponent = DroneDataComponent as TwitterDataComponent;
						UserManager um = new UserManager();
						OAuthTokens oat = Utility.GetOAuthToken(Xml);

						_dataComponent.TwitterUserList = um.GetAllFollowers(oat);

						DroneDataSource.Process(_dataComponent);

						WriteToUsageLogFile("TwitterFollower.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetTwitterFollowers");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.Twitter.Components.TwitterFollower.GetData()");
			}
		}

	}
}
