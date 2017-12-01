using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.Portfolio;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueuePortfolio : BaseQueueComponent<QueueProcessorComponent>
	{
		#region constructors

		public QueuePortfolio()
			: base()
		{
			QueueComponentDataSource = new PortfolioBulkDataSource();
		}

		#endregion

		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;
			string msg = string.Empty;

			try
			{
				msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());

				PortfolioDataType ms = Utility.DeserializeXMLString<PortfolioDataType>(msg);

				if (!Object.Equals(ms, null))
				{
					WriteToUsageLogFile("PortfolioDataType:" + ms.ToString(), string.Format("Executing {0}.{1}", ProcessorName, MethodInfo.GetCurrentMethod().Name), true);
					PortfolioDataComponent dc = new PortfolioDataComponent();
					dc.PortfolioType = ms;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(dc);
					FireMessageProcessedEvent();
				}

				ms = null;
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueuePortfolio.ProcessMessage", "Msg to process: " + msg);

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
