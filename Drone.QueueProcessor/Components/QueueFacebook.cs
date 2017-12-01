using System;
using System.ComponentModel.Composition;
using System.Text;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data.Queue;
using Drone.Entities.Facebook;
using Drone.QueueProcessor.Datasources;
using Drone.Shared;

namespace Drone.QueueProcessor.Components
{
	public class QueueFacebook : BaseQueueComponent<QueueProcessorComponent>
	{
		#region constructors

		public QueueFacebook()
			: base()
		{
			QueueComponentDataSource = new FacebookDataSource();
		}

		#endregion

		#region event handlers

		public override void ProcessMessage(object sender, MessageEventArgs args)
		{
			bool handled = false;

			try
			{
				FacebookDataComponent dc = new FacebookDataComponent();
				string msg = Encoding.UTF8.GetString(args.Message.BodyStream.ToByteArray());
				Page page = Utility.DeserializeXMLString<Page>(msg);

				if (!Object.Equals(page, null))
				{
					dc.FBPage = page;

					handled = true;
					FireMessageProcessingEvent();
					QueueComponentDataSource.Process(dc);
					FireMessageProcessedEvent();
				}
				else
				{
					Demographic<Country> ctry = Utility.DeserializeXMLString<Demographic<Country>>(msg);
					if (!Object.Equals(ctry, null))
					{
						dc.CountryDemographic = ctry;

						handled = true;
						FireMessageProcessingEvent();
						QueueComponentDataSource.Process(dc);
						FireMessageProcessedEvent();
					}
					else
					{
						Demographic<Locale> lcl = Utility.DeserializeXMLString<Demographic<Locale>>(msg);
						if (!Object.Equals(lcl, null))
						{
							dc.LocaleDemographic = lcl;

							handled = true;
							FireMessageProcessingEvent();
							QueueComponentDataSource.Process(dc);
							FireMessageProcessedEvent();
						}
						else
						{
							Demographic<Gender> gndr = Utility.DeserializeXMLString<Demographic<Gender>>(msg);
							if (!Object.Equals(gndr, null))
							{
								dc.GenderDemographic = gndr;

								handled = true;
								FireMessageProcessingEvent();
								QueueComponentDataSource.Process(dc);
								FireMessageProcessedEvent();
							}
						}
					}

				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "QueueFacebook.ProcessMessage");

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
