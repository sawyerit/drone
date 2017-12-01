using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drone.WebAPI.Models
{
	public class API
	{
		public APIType Type { get; set; }
		public string Category { get; set; }
		public string Url { get; set; }
		public string Documentation { get; set; }
		public string UsageExample { get; set; }
	}

	public enum APIType
	{
		GET,
		POST
	}
}