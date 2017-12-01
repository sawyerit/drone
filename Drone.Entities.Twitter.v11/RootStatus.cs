using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drone.Entities.Twitter.v11
{
	public class Metadata
	{
		public string result_type { get; set; }
		public string iso_language_code { get; set; }
	}

	public class User
	{
		public string id { get; set; }
		public string id_str { get; set; }
		public string name { get; set; }
		public string screen_name { get; set; }
		public string location { get; set; }
		public string description { get; set; }
		public string url { get; set; }
		public int? followers_count { get; set; }
		public int? friends_count { get; set; }
		public int? listed_count { get; set; }
		public string created_at { get; set; }
		public int? favourites_count { get; set; }
		public int? utc_offset { get; set; }
		public string time_zone { get; set; }
		public bool? geo_enabled { get; set; }
		public bool? verified { get; set; }
		public int? statuses_count { get; set; }
		public string lang { get; set; }
	}

	public class Status
	{
		public Metadata metadata { get; set; }
		public string created_at { get; set; }
		public object id { get; set; }
		public string id_str { get; set; }
		public string text { get; set; }
		public string source { get; set; }
		public bool? truncated { get; set; }
		public string in_reply_to_status_id { get; set; }
		public string in_reply_to_status_id_str { get; set; }
		public string in_reply_to_user_id { get; set; }
		public string in_reply_to_user_id_str { get; set; }
		public string in_reply_to_screen_name { get; set; }
		public User user { get; set; }
		public string retweet_count { get; set; }
		public string favorite_count { get; set; }
		public bool? favorited { get; set; }
		public bool? retweeted { get; set; }
		public string lang { get; set; }
	}

	public class SearchMetadata
	{
		public long? max_id { get; set; }
		public string max_id_str { get; set; }
		public string next_results { get; set; }
		public string query { get; set; }
		public string refresh_url { get; set; }
		public int? count { get; set; }
		public long? since_id { get; set; }
		public string since_id_str { get; set; }
	}

	public class RootStatus
	{
		public List<Status> statuses { get; set; }
		public SearchMetadata search_metadata { get; set; }
	}
}
