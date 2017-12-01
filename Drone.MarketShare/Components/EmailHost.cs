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
	public class EmailHost : MarketShareBase<MarketShareComponent>
	{
		#region constructors

		[ImportingConstructor]
		public EmailHost()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public EmailHost(IDroneDataSource datasource)
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
					SetNextRunIntervalByNode("emailhost", cont);

					if (XMLUtility.IsEnabled(Xml) && XMLUtility.IsComponentEnabled(Xml, "emailhost"))
					{
						//do work
						WriteToUsageLogFile("EmailHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started EmailHost calls");

						GetAllEmailHosts();

						WriteToUsageLogFile("EmailHost.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed EmailHost calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.EmailHost.GetData()");
			}
		}

		#region internal methods

		/// <summary>
		/// Gets registrars for all companies in our database
		/// </summary>
		internal void GetAllEmailHosts()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				using (Dig dig = new Dig())
				{
					var list = mds.GetAllCompanies(MarketShareTypeBitMaskEnum.EmailHost, XMLUtility.GetIntFromAccountNode(Xml, "emailhost/recordcount"));

					foreach (var company in list)
					{
						try
						{
							MarketShareDataType marketType = new MarketShareDataType();
							string url = Utility.CleanUrl(company.Uri.ToString());

							if (!String.IsNullOrWhiteSpace(url))
							{
								WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

								marketType.Value = dig.GetEmailHostName(url);
								marketType.DomainId = company.DomainId;
								marketType.SampleDate = company.DomainAttributes["SampleDate"];
								marketType.TypeId = (int)MarketShareDataTypeEnum.EmailHost;
								marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.EmailHost;

								if (!string.IsNullOrWhiteSpace(marketType.Value))
								{
									MarketShareDataComponent dc = new MarketShareDataComponent();
									dc.MarketShareType = marketType;
									DroneDataSource.Process(dc);
								}
								else
								{
									Utility.WriteToLogFile(String.Format("SmallBiz_NoEmailHost_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
								}
							}
						}
						catch (Exception e)
						{
							ExceptionExtensions.LogError(e, "EmailHost.GetAllEmailHosts", "Domain: " + company.Uri.ToString());
						}
					}
				}
			}
		}

		#endregion
	}
}
