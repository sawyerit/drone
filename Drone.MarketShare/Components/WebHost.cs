using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Drone.API.Dig;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.MarketShare;
using Drone.MarketShare.Datasources;
using Drone.Shared;

namespace Drone.MarketShare.Components
{
	[Export(typeof(IDroneComponent))]
	public class WebHost : MarketShareBase<MarketShareComponent>
	{
		Guid g = Guid.NewGuid();

		#region constructors

		[ImportingConstructor]
		public WebHost()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public WebHost(IDroneDataSource datasource)
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
						WriteToUsageLogFile("WebHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started WebHost calls");

						GetAllWebHosts();

						WriteToUsageLogFile("WebHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed WebHost calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.WebHost.GetData()");
			}
		}

		#region internal methods


		private void GetAllWebHosts()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				using (Dig dig = new Dig())
				{
					int maxParallel = XMLUtility.GetTextFromAccountNode(Xml, ProcessorName + "/maxparallel").ConvertStringToInt(1);

					Parallel.ForEach(mds.GetAllCompanies(MarketShareTypeBitMaskEnum.WebHost, XMLUtility.GetIntFromAccountNode(Xml, ProcessorName + "/recordcount"))
													, new ParallelOptions { MaxDegreeOfParallelism = maxParallel }
													, (company, state) =>
													{
														try
														{
															if (Context.ShuttingDown) state.Break();

															MarketShareDataType marketType = new MarketShareDataType();
															string url = Utility.CleanUrl(company.Uri.ToString());

															if (!String.IsNullOrWhiteSpace(url))
															{
																WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

																marketType.Value = dig.GetWebHostName(url);
																marketType.DomainId = company.DomainId;
																marketType.SampleDate = company.DomainAttributes["SampleDate"];
																marketType.TypeId = (int)MarketShareDataTypeEnum.WebHost;
																marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.WebHost;
																marketType.UniqueID = g;

																if (!string.IsNullOrWhiteSpace(marketType.Value))
																{
																	MarketShareDataComponent dc = new MarketShareDataComponent();
																	dc.MarketShareType = marketType;
																	DroneDataSource.Process(dc);
																}
																else
																{
																	Utility.WriteToLogFile(String.Format("SmallBiz_NoWebHost_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
																}
															}
														}
														catch (Exception e)
														{
															ExceptionExtensions.LogError(e, "WebHost.GetAllWebHostsParallel", "Domain: " + company.Uri.ToString());
														}
													});
				}
			}
		}


		#endregion
	}
}
