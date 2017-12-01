using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter.v11
{
	public class Place
	{
		public string name { get; set; }
		public string url { get; set; }
		public PlaceType placeType { get; set; }
		public int parentid { get; set; }
		public string countryCode { get; set; }
		public string country { get; set; }
		public int woeid { get; set; }
	}
}
