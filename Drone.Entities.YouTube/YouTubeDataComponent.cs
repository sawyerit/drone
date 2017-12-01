using Drone.Entities.YouTube;
using Drone.Core;

namespace Drone.Entities.YouTube
{
	public class YouTubeDataComponent : BaseDataComponent<YouTubeComponent>
	{
		public Channel YouTubeChannel { get; set; }
		public ChannelVideo SquatterVideo { get; set; }
	}
}
