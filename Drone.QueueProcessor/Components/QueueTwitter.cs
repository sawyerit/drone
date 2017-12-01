using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.Twitter.v11;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueueTwitter : BaseQueueComponent<QueueProcessorComponent>
	{
		#region constructors

		public QueueTwitter()
			: base()
		{
			QueueComponentDataSource = new TwitterDataSource();
		}

		#endregion

		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;
			try
			{
				string msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());
				KeywordStatus ks = Utility.DeserializeXMLString<KeywordStatus>(msg);

				if (!Object.Equals(ks, null))
				{					
					TwitterDataComponent tdc = new TwitterDataComponent();
					tdc.KeywordStatus = ks;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(tdc);
					FireMessageProcessedEvent();
				}

				List<User> users = Utility.DeserializeXMLString<List<User>>(msg);
				if (!Object.Equals(null, users))
				{
					TwitterDataComponent tdc = new TwitterDataComponent();
					tdc.TwitterUserList = users;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(tdc);
					FireMessageProcessedEvent();
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueueTwitter.ProcessMessage");

				if (Utility.IsCriticalDBError(e))
				{
					FireShuttingDownEvent();
				}

				if (handled)
					FireMessageProcessedEvent();
			}
		}

		#endregion


	}
}
