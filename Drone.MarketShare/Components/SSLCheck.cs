using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Drone.API.Dig;
using Drone.API.Dig.Ssl;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.MarketShare;
using Drone.MarketShare.Datasources;
using Drone.Shared;

namespace Drone.MarketShare.Components
{
	[Export(typeof(IDroneComponent))]
	public class SSLCheck : MarketShareBase<MarketShareComponent>
	{
		#region constructors

		[ImportingConstructor]
		public SSLCheck()
			: base()
		{
			DroneDataSource = new MarketShareDataSource();
		}

		public SSLCheck(IDroneDataSource datasource)
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
						WriteToUsageLogFile("SSLCheck.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started SSLCheck calls");

						DoSSLCheck();

						WriteToUsageLogFile("SSLCheck.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed SSLCheck calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.SSLCheck.GetData()");
			}
		}

		#region internal methods


		private void DoSSLCheck()
		{
			MarketShareDataSource mds = DroneDataSource as MarketShareDataSource;
			if (!Object.Equals(null, mds))
			{
				int maxParallel = XMLUtility.GetTextFromAccountNode(Xml, ProcessorName + "/maxparallel").ConvertStringToInt(1);
				Dig dig = new Dig();

				Parallel.ForEach(mds.GetAllCompanies(MarketShareTypeBitMaskEnum.SSLIssuer, XMLUtility.GetIntFromAccountNode(Xml, ProcessorName + "/recordcount"))
												, new ParallelOptions { MaxDegreeOfParallelism = maxParallel }
												, (company, state) =>
												{
													try
													{
														if (Context.ShuttingDown) state.Break();

														MarketShareDataType marketType = new MarketShareDataType();
														string url = Utility.CleanUrl(company.Uri.ToString());
														WriteToUsageLogFile("Domain:" + url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);

														if (!String.IsNullOrWhiteSpace(url))
														{
															SSLCert cert = dig.GetSSLVerification(url);
															if (!Object.Equals(cert, null))
															{
																MarketShareDataComponent dc = new MarketShareDataComponent();

																//Issuer
																marketType.Value = cert.FixedName;
																marketType.DomainId = company.DomainId;
																marketType.SampleDate = company.DomainAttributes["SampleDate"];
																marketType.TypeId = (int)MarketShareDataTypeEnum.SSLIssuer;
																marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.SSLIssuer;
																dc.MarketShareType = marketType;
																DroneDataSource.Process(dc);

																//CertType
																marketType.Value = cert.SubjectType;
																marketType.TypeId = (int)MarketShareDataTypeEnum.CertificateType;
																marketType.BitMaskId = (int)MarketShareTypeBitMaskEnum.CertificateType;
																dc.MarketShareType = marketType;
																DroneDataSource.Process(dc);

															}
															else
															{
																Utility.WriteToLogFile(String.Format("SmallBiz_NoSSLInfo_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: ,{0}", url));
															}
														}
													}
													catch (Exception e)
													{
														ExceptionExtensions.LogError(e, "SSLCheck.DoSSLCheck", "Domain: " + company.Uri.ToString());
													}
												});
			}
		}


		#endregion

	}
}
