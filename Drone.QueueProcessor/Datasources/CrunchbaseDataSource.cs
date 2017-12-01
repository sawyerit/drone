using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Data;
using Drone.Entities.Crunchbase;
using Drone.QueueProcessor.Components;
using Drone.Shared;

namespace Drone.QueueProcessor.Datasources
{
	public class CrunchbaseDataSource : BaseDatasource<QueueProcessorComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			CrunchbaseDataComponent crunchComponent = component as CrunchbaseDataComponent;
			if (!Object.Equals(crunchComponent, null))
			{
				SaveCompany(crunchComponent.CompanyLocal);
			}
		}

		//allow nulls, the proc should NOT overwrite current data if a null is passed in
		private void SaveCompany(CompanyRoot companyRoot)
		{
			try
			{
				string investors = GetInvestors(companyRoot);
				DateTime dt = DateTime.MinValue;                
				
                companyRoot.founded_year = companyRoot.founded_year ?? 1900;
                companyRoot.founded_year = companyRoot.founded_year.Value < 1753 ? 1900 : companyRoot.founded_year;
				companyRoot.founded_month = companyRoot.founded_month ?? 1;
                companyRoot.founded_month = companyRoot.founded_month.Value > 12 ? 12 : companyRoot.founded_month;
				companyRoot.founded_day = companyRoot.founded_day ?? 1;

                DateTime.TryParse(string.Format("{0}-{1}-{2}", companyRoot.founded_year.Value, companyRoot.founded_month.Value, companyRoot.founded_day.Value), out dt);
				
				DataFactory.ExecuteNonQuery("SmallBusinessTrackingInsUpd",
																		new KeyValuePair<string, object>("@Domain", companyRoot.homepage_url),
																		new KeyValuePair<string, object>("@CompanyName", companyRoot.name.Trim()),
																		new KeyValuePair<string, object>("@DateFounded", dt.Date),
																		new KeyValuePair<string, object>("@WebHost", companyRoot.records.WebHost.Trim()),
																		new KeyValuePair<string, object>("@EmailHost", companyRoot.records.EmailHost.Trim()),
																		new KeyValuePair<string, object>("@DNSHost", companyRoot.records.DNSHost.Trim()),
																		new KeyValuePair<string, object>("@Registrar", companyRoot.records.Registrar.Trim()),
																		new KeyValuePair<string, object>("@SSLIssuer", companyRoot.records.SSLIssuer.Trim()),
																		new KeyValuePair<string, object>("@CertificateType", companyRoot.records.CertificateType.Trim()),
																		new KeyValuePair<string, object>("@NumberOfEmployees", companyRoot.number_of_employees ?? 0),
																		new KeyValuePair<string, object>("@TotalFunding", Object.Equals(null, companyRoot.total_money_raised) ? "None" : companyRoot.total_money_raised.Trim()),
																		new KeyValuePair<string, object>("@Investors", investors));
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deadlocked"))
				{
					SaveCompany(companyRoot);
					ExceptionExtensions.LogInformation("CrunchbaseDataSource.SaveCompany()", "Deadlock encountered, trying again");
				}
				else if (e.Message.Contains("Timeout expired"))
				{
					Thread.Sleep(5000);
					SaveCompany(companyRoot);
				}
				else
				{
					ExceptionExtensions.LogError(e, "CrunchbaseDataSource.SaveCompany", "Name: " + companyRoot.name);

					//if tempdb full or other critical db error, re-throw
					if (Utility.IsCriticalDBError(e)) throw;
				}
			}
		}

		private static string GetInvestors(CompanyRoot companyRoot)
		{
			List<string> investors = new List<string>();

			foreach (var round in companyRoot.funding_rounds)
			{
				foreach (var inv in round.investments)
				{
					if (!Object.Equals(inv.company, null) && !string.IsNullOrEmpty(inv.company.name) && !investors.Exists(d => d == inv.company.name))
						investors.Add(inv.company.name);

					if (!Object.Equals(inv.financial_org, null) && !string.IsNullOrEmpty(inv.financial_org.name) && !investors.Exists(d => d == inv.financial_org.name))
						investors.Add(inv.financial_org.name);

					if (!Object.Equals(inv.person, null) && !string.IsNullOrEmpty(inv.person.first_name) && !investors.Exists(d => d == inv.person.first_name))
						investors.Add(string.Format("{0} {1}", inv.person.first_name, inv.person.last_name));
				}
			}

			if (Object.Equals(null, investors) || investors.Count <= 0)
				return "None";
			else
				return string.Join(",", investors.OrderBy(item => item).ToList());
		}
	}
}
