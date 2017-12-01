using System;
using System.Collections.Generic;

namespace Drone.Entities.WebAPI
{
	public class Domain
	{
		public Uri Uri { get; set; }
		public int DomainId { get; set; }
		public string DomainName { get; set; }
		public string ShopperID { get; set; }
		/// <summary>
		/// History of what was already saved for this domain
		/// </summary>
		public Dictionary<string, string> DomainAttributes { get; set; }
	}
}