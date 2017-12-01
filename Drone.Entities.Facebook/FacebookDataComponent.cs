using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Core;
using Drone.Entities.Facebook;

namespace Drone.Entities.Facebook
{
	public class FacebookDataComponent : BaseDataComponent<FacebookComponent>
	{
		public Page FBPage { get; set; }
		public Post FBPost { get; set; }
		public Demographic<Country> CountryDemographic { get; set; }
		public Demographic<Locale> LocaleDemographic { get; set; }
		public Demographic<Gender> GenderDemographic { get; set; }
	}
}
