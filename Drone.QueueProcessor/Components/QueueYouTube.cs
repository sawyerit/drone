using System;
using System.ComponentModel.Composition;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.YouTube;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueueYouTube : BaseQueueComponent<QueueProcessorComponent>
	{
		#region constructors

		public QueueYouTube()
			: base()
		{
			QueueComponentDataSource = new YouTubeDataSource();
		}

		#endregion

		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;
			try
			{
				string msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());
				Channel ch = Utility.DeserializeXMLString<Channel>(msg);

				if (!Object.Equals(ch, null))
				{
					YouTubeDataComponent ytdc = new YouTubeDataComponent();
					ytdc.YouTubeChannel = ch;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(ytdc);
					FireMessageProcessedEvent();
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueueYouTube.ProcessMessage");

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
