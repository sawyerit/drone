using System;
using System.Collections.Generic;
using System.Text;
using Drone.Core;
using Drone.Crunchbase.Components;
using Drone.Entities.WebAPI;
using Drone.Core.Interfaces;
using Drone.Shared;
using Drone.Entities.Crunchbase;

namespace Drone.Crunchbase.Datasources
{
	public class CrunchbaseTestDataSource : BaseDatasource<CrunchbaseComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			CrunchbaseDataComponent comp = component as CrunchbaseDataComponent;
			if (!Object.Equals(comp.CompanyLocal, null))
				Utility.WriteToLogFile(String.Format("Crunchbase_TestDataRun_{0:M_d_yyyy}", DateTime.Today) + ".log", BuildMessage(comp.CompanyLocal));
		}

		private static string BuildMessage(Drone.Entities.Crunchbase.CompanyRoot companyRoot)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("name: " + companyRoot.name);
			sb.Append(", perm:" + companyRoot.permalink);
			sb.Append(", domain: " + companyRoot.homepage_url);
			if (!Object.Equals(null, companyRoot.records))
			{
				sb.Append(", reg:" + companyRoot.records.Registrar);
				sb.Append(", webhost:" + companyRoot.records.WebHost);
				sb.Append(", dnshost:" + companyRoot.records.DNSHost);
				sb.Append(", emailhost:" + companyRoot.records.EmailHost);
				sb.Append(", certType:" + companyRoot.records.CertificateType);
				sb.Append(", SSL:" + companyRoot.records.SSLIssuer);
			}
			sb.Append(", " + DateTime.Now);

			return sb.ToString();
		}

		public List<Domain> AllCompanies(int typeID, int rows)
		{
			Dictionary<string, string> attrs = new Dictionary<string, string>();
			attrs.Add("SampleDate", DateTime.Now.ToString());
			List<Domain> allCompanies = new List<Domain>();

			allCompanies.Add(new Domain { DomainId = 1, Uri = new Uri("http://godaddy.com"), DomainAttributes = attrs });
			allCompanies.Add(new Domain { DomainId = 2, Uri = new Uri("http://microsoft.com"), DomainAttributes = attrs });
			allCompanies.Add(new Domain { DomainId = 3, Uri = new Uri("http://tattly.com"), DomainAttributes = attrs });
			allCompanies.Add(new Domain { DomainId = 4, Uri = new Uri("http://saferwholesale.com"), DomainAttributes = attrs });
			allCompanies.Add(new Domain { DomainId = 5, Uri = new Uri("http://courtneytidman.co.uk"), DomainAttributes = attrs });
			return allCompanies;
		}
	}
}
