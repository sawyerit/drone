using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Drone.API.Crunchbase;
using Drone.API.Dig;
using Drone.API.Dig.Ssl;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Crunchbase.Datasources;
using Drone.Entities.Crunchbase;
using Drone.Shared;

namespace Drone.Crunchbase.Components
{
	[Export(typeof(IDroneComponent))]
	public class Crunch : BaseComponent<CrunchbaseComponent>
	{
		CompanyManager compManager = new CompanyManager();

		#region constructors

		[ImportingConstructor]
		public Crunch()
			: base()
		{
			DroneDataSource = new CrunchbaseDataSource();
		}

		public Crunch(IDroneDataSource datasource)
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

					if (XMLUtility.IsEnabled(Xml)
								&& XMLUtility.IsComponentEnabled(Xml, ProcessorName)
								&& (int)DateTime.Today.DayOfWeek == (int)Enum.Parse(typeof(DayOfWeek), XMLUtility.GetTextFromAccountNode(Xml, ProcessorName + "/interval"), true)
							|| Utility.FileExists("Crunchbase_AllCompanies.txt")) //if this file exists, it means the service failed and is restarting.
					{
						//do work
						WriteToUsageLogFile("Crunchbase.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started Crunchbase calls");

						GetAllCompanies();

						WriteToUsageLogFile("Crunchbase.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed Crunchbase calls");
					}

					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.SmallBusinessTracking.Components.Crunchbase.GetData()");
			}
		}

		#region internal methods

		/// <summary>
		/// Gets all companies from crunchbase (115k+)
		/// Long running process takes about 40 hours. All Companies are written to a file and deleted as processed.
		/// If the service has to recover from a crash it will start again where it left off.
		/// </summary>
		internal void GetAllCompanies()
		{
			try
			{
				string fileName = "Crunchbase_AllCompanies.txt";
				if (Utility.FileExists(fileName))
				{
					Process(fileName);
				}
				else
				{
					List<Company> allCompanies = compManager.GetAllCompanies();

					if (allCompanies.Count > 0)
					{
						//write all companies to a file instead of holding in memory
						Utility.WriteToLogFile(fileName, allCompanies.Select(d => d.name + "|" + d.permalink).ToArray());
						allCompanies = null;

						Process(fileName);
					}
					else
					{
						WriteToUsageLogFile("Crunchbase.GetAllCompanies()", "Crunchbase call returned no companies.  Trying again in 10 seconds.");
						Thread.Sleep(10000);
						GetAllCompanies();
					}
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Crunchbase.GetAllCompanies");
			}
		}

		internal void Process(string fileName)
		{
			Dig digMgr = new Dig();
			List<string> tempLines;

			while ((tempLines = Utility.ReadLinesFromFile(fileName, 100, true)).Count() > 0)
			{
				int maxParallel = XMLUtility.GetTextFromAccountNode(Xml, ProcessorName + "/maxparallel").ConvertStringToInt(1);

				Parallel.ForEach(tempLines, new ParallelOptions { MaxDegreeOfParallelism = maxParallel }, (tempLine) =>
				{
					try
					{
						if (!String.IsNullOrWhiteSpace(tempLine))
						{
							string[] companyEntry = tempLine.Split('|');
							CompanyRoot cr = GetFullCompany(companyEntry[1], digMgr);

							//Add to MSMQ, add will call receive and then ProcessMessage below when to add to DB asynchronously
							if (!Object.Equals(null, cr) && !string.IsNullOrEmpty(cr.homepage_url))
							{
								WriteToUsageLogFile("Domain:" + cr.homepage_url, string.Format("Executing {0}.{1}", ComponentTypeName, MethodInfo.GetCurrentMethod().Name), true);
								CrunchbaseDataComponent cdc = new CrunchbaseDataComponent();
								cdc.CompanyLocal = cr;
								DroneDataSource.Process(cdc);
							}
							else
							{
								if (Object.Equals(null, cr) && companyEntry.Length < 3)
									Utility.AddLineToFile(fileName, tempLine + "|retry");
							}
							cr = null;
						}
					}
					catch (Exception e)
					{
						ExceptionExtensions.LogError(e, "Crunchbase.Process", "Permalink: " + tempLine);
					}
				});
			}
		}

		internal CompanyRoot GetFullCompany(string permalinkName, Dig digMgr)
		{
			CompanyRoot company = compManager.GetFullCompany(permalinkName);

			if (!Object.Equals(company, null) && !Object.Equals(null, company.homepage_url))
			{
				Records r = new Records();
				company.homepage_url = Utility.CleanUrl(company.homepage_url);
				IPAddress ip = digMgr.GetIPAddress(company.homepage_url);
				if (!Object.Equals(null, ip))
					company.ip_address = ip.ToString();
				SSLCert cert = digMgr.GetSSLVerification(company.homepage_url);

				if (!Object.Equals(cert, null))
				{
					r.SSLIssuer = cert.FixedName;
					r.CertificateType = cert.SubjectType;
				}
				r.WebHost = digMgr.GetWebHostName(company.homepage_url);
				r.EmailHost = digMgr.GetEmailHostName(company.homepage_url);
				r.DNSHost = digMgr.GetDNSHostName(company.homepage_url);
				r.Registrar = digMgr.GetRegistrar(company.homepage_url);

				company.records = r;
			}

			return company;
		}

		#endregion
	}
}
