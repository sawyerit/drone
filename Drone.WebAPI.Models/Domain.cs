using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drone.WebAPI.Models
{
	public class Domain
	{
		public Uri Uri { get; set; }
		public int DomainId { get; set; }
		/// <summary>
		/// History of what was already saved for this domain
		/// </summary>
		public Dictionary<string, string> DomainAttributes { get; set; }
	}
}