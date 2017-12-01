using System;
using System.Collections.Generic;
using Drone.Core;

namespace Drone.Twitter
{
	public sealed class TwitterContext : BaseContext
	{
		public Dictionary<int, long> LatestTweetIDs = new Dictionary<int, long>();
		public Dictionary<int, long> prevRunLatestTweetIDs = new Dictionary<int, long>();
	}
}
