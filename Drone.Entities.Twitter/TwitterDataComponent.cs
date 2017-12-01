using Drone.Core;
using System.Collections.Generic;
using Drone.Entities.Twitter;

namespace Drone.Entities.Twitter
{
	public class TwitterDataComponent : BaseDataComponent<TwitterComponent>
	{
		public List<Place> TwitterAvailablePlaces { get; set; }
		public List<Mention> TwitterMentions { get; set; }
		public List<DirectMessage> TwitterDirectMessages { get; set; }
		public KeywordStatus KeywordStatus { get; set; }
		public bool SaveFailure;

		public List<List<TrendRoot>> TrendRootList { get; set; }

		public List<User> TwitterUserList { get; set; }
	}
}
