using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Drone.API.MarketAnalysis;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.MarketShare;
using Drone.MarketShare.Datasources;
using Drone.Shared;

namespace Drone.MarketShare.Components
{
	[Export(typeof(IDroneComponent))]
	public class MarketShareBuilder : MarketShareBase<MarketShareComponent>
	{
		Guid g = Guid.NewGuid();

		#region constructors

		[ImportingConstructor]
		public MarketShareBuilder()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public MarketShareBuilder(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
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
				Context = cont;

				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);
					SetNextRunIntervalByNode(ProcessorName, cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, ProcessorName))
					{
						//do work
						WriteToUsageLogFile("MarketShareBuilder.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started MarketShareBuilder calls");

						GetBuilders();

						WriteToUsageLogFile("MarketShareBuilder.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed MarketShareBuilder calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
				GC.Collect();
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.MarketShareBuilder.GetData()");
			}
		}

		#region internal methods

		private void GetBuilders()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				int maxParallel = XMLUtility.GetTextFromAccountNode(Xml, ProcessorName + "/maxparallel").ConvertStringToInt(1); 

				Parallel.ForEach(mds.GetAllCompanies(MarketShareTypeBitMaskEnum.SiteBuilder, XMLUtility.GetIntFromAccountNode(Xml, ProcessorName + "/recordcount"))
												, new ParallelOptions { MaxDegreeOfParallelism = maxParallel }
												, (company, state) =>
				{
					try
					{
						if (Context.ShuttingDown) state.Break();

						MarketShareDataType marketDataType = new MarketShareDataType();
						string url = Utility.CleanUrl(company.Uri.ToString());
						WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

						if (!String.IsNullOrWhiteSpace(url))
						{
							marketDataType.Value = MarketShareEngine.Instance.SiteBuilder(url);
							marketDataType.DomainId = company.DomainId;
							marketDataType.SampleDate = company.DomainAttributes["SampleDate"];
							marketDataType.TypeId = (int)MarketShareDataTypeEnum.SiteBuilder;
							marketDataType.BitMaskId = (int)MarketShareTypeBitMaskEnum.SiteBuilder;
							marketDataType.UniqueID = g;

							if (!string.IsNullOrWhiteSpace(marketDataType.Value))
							{
								MarketShareDataComponent dc = new MarketShareDataComponent();
								dc.MarketShareType = marketDataType;
								DroneDataSource.Process(dc);
							}
							else
							{
								Utility.WriteToLogFile(String.Format("SmallBiz_NoBuilder_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
							}
						}
					}
					catch (Exception e)
					{
						ExceptionExtensions.LogError(e, "MarketShareBuilder.GetBuilders", "Domain: " + company.Uri.ToString());
					}
				});
			}
		}

		#endregion
	}
}
