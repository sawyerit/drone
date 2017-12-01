using Drone.Entities.YouTube;

namespace Drone.WebAPI.Interfaces
{
	public interface IYouTubeService
	{
		Channel Create(Channel value);
		Channel GetChannel(string user);		
		ChannelVideo Get(string id);
	}
}
