using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading;
using Drone.API.Dig;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.MarketShare;
using Drone.MarketShare.Datasources;
using Drone.Shared;

namespace Drone.MarketShare.Components
{
	[Export(typeof(IDroneComponent))]
	public class WhoIs : MarketShareBase<MarketShareComponent>
	{
		#region constructors

		[ImportingConstructor]
		public WhoIs()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public WhoIs(IDroneDataSource datasource)
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
						WriteToUsageLogFile("WhoIs.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started WhoIs calls");

						GetAllRegistrars();

						WriteToUsageLogFile("WhoIs.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed WhoIs calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.WhoIs.GetData()");
			}
		}

		#region internal methods

		/// <summary>
		/// Gets registrars for all companies in our database
		/// </summary>
		internal void GetAllRegistrars()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				using (Dig dig = new Dig())
				{
					var list = mds.GetAllCompanies(MarketShareTypeBitMaskEnum.Registrar, XMLUtility.GetIntFromAccountNode(Xml, ProcessorName + "/recordcount"));

					foreach (var company in list)
					{
						try
						{
							MarketShareDataType marketType = new MarketShareDataType();
							string url = Utility.CleanUrl(company.Uri.ToString());
							WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

							if (!String.IsNullOrWhiteSpace(url))
							{
								marketType.Value = dig.GetRegistrar(url);
								marketType.DomainId = company.DomainId;
								marketType.SampleDate = company.DomainAttributes["SampleDate"];
								marketType.TypeId = (int)MarketShareDataTypeEnum.Registrar;
								marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.Registrar;

								if (!string.IsNullOrWhiteSpace(marketType.Value))
								{
									MarketShareDataComponent dc = new MarketShareDataComponent();
									dc.MarketShareType = marketType;
									DroneDataSource.Process(dc);
								}
								else
								{
									Utility.WriteToLogFile(String.Format("SmallBiz_NoRegistrar_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
								}
							}
						}
						catch (Exception e)
						{
							ExceptionExtensions.LogError(e, "WhoIs.GetAllRegistrars", "Domain: " + company.Uri.ToString());
						}
					}
				}
			}
		}

		#endregion
	}
}
