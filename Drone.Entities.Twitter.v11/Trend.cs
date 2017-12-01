using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter.v11
{
	public class Trend
	{
		public string query { get; set; }
		public string name { get; set; }
		public object promoted_content { get; set; }
		public object events { get; set; }
		public string url { get; set; }
	}
}
