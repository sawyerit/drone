using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.Crunchbase;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueueCrunchbase : BaseQueueComponent<QueueProcessorComponent>
	{
		private MessageDelegates.MessageReceivedEventHandler MessageReceived;

		#region constructors

		public QueueCrunchbase()
			: base()
		{
			QueueComponentDataSource = new CrunchbaseDataSource();
		}

		#endregion


		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;

			try
			{
				string msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());
				CompanyRoot cr = Utility.DeserializeXMLString<CompanyRoot>(msg);
				if (!Object.Equals(cr, null))
				{
					WriteToUsageLogFile("Domain:" + cr.homepage_url, string.Format("Executing {0}.{1}", ProcessorName, MethodInfo.GetCurrentMethod().Name), true);

					CrunchbaseDataComponent dc = new CrunchbaseDataComponent();
					dc.CompanyLocal = cr;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(dc);
					FireMessageProcessedEvent();
				}

				cr = null;
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueueCrunchbase.ProcessMessage");
				
				if (Utility.IsCriticalDBError(e))
				{
					FireShuttingDownEvent();
				}

				if (handled) //if false, processor was never incremented, no need to decrement
					FireMessageProcessedEvent();
			}
		}

		#endregion
	}
}
