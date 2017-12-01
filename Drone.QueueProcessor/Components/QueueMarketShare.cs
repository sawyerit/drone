using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.MarketShare;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueueMarketShare : BaseQueueComponent<QueueProcessorComponent>
	{
		#region constructors

		public QueueMarketShare()
			: base()
		{
			QueueComponentDataSource = new MarketShareBulkDataSource();
		}

		#endregion

		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;

			try
			{
				string msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());
				MarketShareDataType ms = Utility.DeserializeXMLString<MarketShareDataType>(msg);

				if (!Object.Equals(ms, null))
				{
					WriteToUsageLogFile("MarketShareDataType:" + ms.ToString(), string.Format("Executing {0}.{1}", ProcessorName, MethodInfo.GetCurrentMethod().Name), true);
					MarketShareDataComponent dc = new MarketShareDataComponent();
					dc.MarketShareType = ms;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(dc);
					FireMessageProcessedEvent();
				}

				ms = null;
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueueMarketShare.ProcessMessage");

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
