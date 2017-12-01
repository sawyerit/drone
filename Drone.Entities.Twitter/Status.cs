using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter
{
	public class Status
	{
		public string created_at { get; set; }
		public string from_user { get; set; }
		public int? from_user_id { get; set; }
		public string from_user_id_str { get; set; }
		public string from_user_name { get; set; }
		public Geo geo { get; set; }
		public object id { get; set; }
		public string id_str { get; set; }
		public string iso_language_code { get; set; }
		public Metadata metadata { get; set; }
		public string profile_image_url { get; set; }
		public string source { get; set; }
		public string text { get; set; }
		public string to_user { get; set; }
		public int? to_user_id { get; set; }
		public string to_user_id_str { get; set; }
		public string to_user_name { get; set; }
	}
}
