using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drone.Entities.Portfolio
{
    public class PortfolioFullDataType : PortfolioDataType
    {
			public int ShopperID { get; set; }
			public int PrivateLabelID { get; set; }
			public string DomainName { get; set; }
			public DateTime LastCrawlDate { get; set; }
			public Dictionary<string, string> Attributes { get; set; }
			public Dictionary<string, string> Verticals { get; set; }
			public Dictionary<string, string> Social { get; set; }

			public override string ToString()
			{
				//todo: return attributes here?
				return string.Format("DomainID:{0} DomainName:{1} ShopperID: {2}", rptGDDomainsId, DomainName, ShopperID);
			}
    }
}
