using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drone.Entities.Portfolio
{
    public class PortfolioDataType
    {
			public int TypeId { get; set; }
			public int rptGDDomainsId { get; set; }
			public int DomainID { get; set; }
			public string Value { get; set; }
			public string ShopperID { get; set; }
			public Guid UniqueID { get; set; }

			public override string ToString()
			{
				return string.Format("Type:{0}, DomainID:{1}, Value:{2}, ShopperID:{3}", TypeId, rptGDDomainsId, Value, ShopperID);
			}
    }
}
