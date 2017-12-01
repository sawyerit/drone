using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Crunchbase
{
	public class Company
	{
		public string name { get; set; }
		public string permalink { get; set; }
		public int total { get; set; }
		public int per_page { get; set; }
		public int current_page { get; set; }
	}
}
