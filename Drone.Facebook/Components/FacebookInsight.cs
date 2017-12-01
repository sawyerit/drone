using System;
using System.ComponentModel.Composition;
using System.Threading;
using Drone.API.Facebook;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Facebook;
using Drone.Shared;


namespace Drone.Facebook.Components
{
	[Export(typeof(IDroneComponent))]
	public class FacebookInsight : BaseComponent<FacebookComponent>
	{
		public Insight FBIO { get; set; }

		#region Constructors

		[ImportingConstructor]
		public FacebookInsight()
			: base()
		{
			DroneDataSource = null; 
		}

		public FacebookInsight(IDroneDataSource datasource)
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

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						WriteToUsageLogFile("FacebookInsight.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetInsightInfo");

						string accountId = XMLUtility.GetTextFromAccountNode(Xml, "id");
						string accessToken = XMLUtility.GetTextFromAccountNode(Xml, "accesstoken");
						FBIO = new Graph().GetInsightInfo(accountId, accessToken);

						WriteToUsageLogFile("FacebookInsight.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetInsightInfo");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "FacebookInsight.GetData()");
			}

		}
	}
}
