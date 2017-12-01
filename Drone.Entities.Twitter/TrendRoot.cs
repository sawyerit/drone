using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter
{
	public class TrendRoot
	{
		public string created_at { get; set; }
		public string as_of { get; set; }
		public List<Trend> trends { get; set; }
		public List<Location> locations { get; set; }
	}
}
