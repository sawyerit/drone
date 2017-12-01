using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Xml;
using Drone.API.YouTube;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.WebAPI;
using Drone.Entities.YouTube;
using Drone.Shared;
using Drone.YouTube.Datasources;

namespace Drone.YouTube.Components
{
	[Export(typeof(IDroneComponent))]
	public class YouTube : BaseComponent<YouTubeComponent>
	{
		#region constructors

		[ImportingConstructor]
		public YouTube()
			: base()
		{
			DroneDataSource = new YouTubeDataSource();
		}

		public YouTube(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
		}

		#endregion

		public override void GetData(object context)
		{
			try
			{
				BaseContext cont = context as BaseContext;
				Context = cont;

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						WriteToUsageLogFile("YouTube.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetChannelData");

						GetSquatterData();
						GetChannelData();

						WriteToUsageLogFile("YouTube.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetChannelData");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "YouTube.GetData()");
			}

		}

		private void GetChannelData()
		{
			YouTubeDataSource yds = DroneDataSource as YouTubeDataSource;
			if (!Object.Equals(null, yds))
			{
				foreach (Competitor account in yds.GetCompetitorAccounts())
				{
					if (!String.IsNullOrEmpty(account.YouTubeAccount))
					{
						ChannelManager cManager = new ChannelManager(new KeyValuePair<int, string>(account.ID, account.YouTubeAccount)
																												, Utility.ApplicationName
																												, XMLUtility.GetTextFromAccountNode(Xml, "token/devkey"));
						Channel chan = cManager.GetUserChannel();

						if (!Object.Equals(chan, null))
						{
							YouTubeDataComponent ydc = new YouTubeDataComponent();
							ydc.YouTubeChannel = chan;
							DroneDataSource.Process(ydc);
						}
						else
							Utility.WriteToLogFile(String.Format("YouTube_ChannelNull_{0:M_d_yyyy}", DateTime.Today) + ".log", "No account info for: " + account.YouTubeAccount);
					}
				}
			}
		}

		private void GetSquatterData()
		{
			ChannelManager cManager = new ChannelManager();
			Channel chanVids = cManager.GetSquatterVideos(GetAllSquatterVideoIds());

			if (!Object.Equals(chanVids, null))
			{
				YouTubeDataComponent ydc = new YouTubeDataComponent();
				ydc.YouTubeChannel = chanVids;
				DroneDataSource.Process(ydc);
			}
			else
				Utility.WriteToLogFile(String.Format("YouTube_SquattersNull_{0:M_d_yyyy}", DateTime.Today) + ".log", "No videos found");
		}

		private Collection<string> GetAllSquatterVideoIds()
		{
			Collection<string> ids = new Collection<string>();

			foreach (XmlNode node in XMLUtility.GetNodeFromXml(Xml, "accounts/account/squatters").ChildNodes)
				ids.Add(node.InnerText);

			return ids;
		}
	}
}
