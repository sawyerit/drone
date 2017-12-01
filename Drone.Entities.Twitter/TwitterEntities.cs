using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Entities.Twitter
{
	public class TwitterEntities
	{
		public object[] hashtags { get; set; }
		public object[] urls { get; set; }
		public object[] user_mentions { get; set; }
	}
}
