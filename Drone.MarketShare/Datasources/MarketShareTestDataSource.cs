using Drone.Core;
using Drone.Core.Interfaces;
using Drone.MarketShare.Components;
using Drone.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Drone.Entities.MarketShare;
using Drone.Entities.WebAPI;

namespace Drone.MarketShare.Datasources
{
	public class MarketShareTestDataSource : BaseDatasource<MarketShareComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			MarketShareDataComponent comp = component as MarketShareDataComponent;
			if (!Object.Equals(comp.MarketShareType, null))
				//For now do nothing, this is a blank datasource for unit testing.
				Utility.WriteToLogFile(String.Format("MarketShare_TestDataRun_{0:M_d_yyyy}", DateTime.Today) + ".log", BuildMarketShareMessage(comp.MarketShareType));
		}

		private static string BuildMarketShareMessage(MarketShareDataType marketShareDataType)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Domain ID: " + marketShareDataType.DomainId);
			sb.Append(", TypeID:" + marketShareDataType.TypeId);
			sb.Append(", value: " + marketShareDataType.Value);			
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
