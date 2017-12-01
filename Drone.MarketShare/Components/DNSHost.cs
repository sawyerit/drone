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
	public class DNSHost : MarketShareBase<MarketShareComponent>
	{
		#region constructors

		[ImportingConstructor]
		public DNSHost()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public DNSHost(IDroneDataSource datasource)
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
					SetNextRunIntervalByNode("dnshost", cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, "dnshost"))
					{
						//do work
						WriteToUsageLogFile("DNSHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started DNSHost calls");

						GetAllDNSHosts();

						WriteToUsageLogFile("DNSHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed DNSHost calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.MarketShare.Components.DNSHost.GetData()");
			}
		}

		#region internal methods

		/// <summary>
		/// Gets registrars for all companies in our database
		/// </summary>
		internal void GetAllDNSHosts()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				using (Dig dig = new Dig())
				{
					foreach (var company in mds.GetAllCompanies(MarketShareTypeBitMaskEnum.DNSHost, XMLUtility.GetIntFromAccountNode(Xml, "dnshost/recordcount")))
					{
						try
						{
							MarketShareDataType marketType = new MarketShareDataType();
							string url = Utility.CleanUrl(company.Uri.ToString());
							WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

							if (!String.IsNullOrWhiteSpace(url))
							{
								marketType.Value = dig.GetDNSHostName(url);
								marketType.DomainId = company.DomainId;
								marketType.TypeId = (int)MarketShareDataTypeEnum.DNSHost;
								marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.DNSHost;
								marketType.SampleDate = company.DomainAttributes["SampleDate"];

								if (!string.IsNullOrWhiteSpace(marketType.Value))
								{
									MarketShareDataComponent dc = new MarketShareDataComponent();
									dc.MarketShareType = marketType;
									DroneDataSource.Process(dc);
								}
								else
								{
									Utility.WriteToLogFile(String.Format("SmallBiz_NoDNSHost_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
								}
							}
						}
						catch (Exception e)
						{
							ExceptionExtensions.LogError(e, "DNSHost.GetAllDNSHosts", "Domain: " + company.Uri.ToString());
						}
					}
				}
			}
		}

		#endregion
	}
}
