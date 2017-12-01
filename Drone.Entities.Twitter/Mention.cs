using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter
{
	public class Mention
	{
		public int? in_reply_to_user_id { get; set; }
		public string in_reply_to_user_id_str { get; set; }
		public object contributors { get; set; }
		public long? in_reply_to_status_id { get; set; }
		public string created_at { get; set; }
		public TwitterEntities entities { get; set; }
		public bool? favorited { get; set; }
		public bool? truncated { get; set; }
		public object place { get; set; }
		public object geo { get; set; }
		public int? retweet_count { get; set; }
		public string in_reply_to_screen_name { get; set; }
		public object coordinates { get; set; }
		public string source { get; set; }
		public bool? retweeted { get; set; }
		public object id { get; set; }
		public string in_reply_to_status_id_str { get; set; }
		public string id_str { get; set; }
		public string text { get; set; }
		public bool? possibly_sensitive { get; set; }
		public User user { get; set; }
	}
}
