
namespace Drone.Entities.Twitter.v11
{
	public class FollowerId
	{
		public string next_cursor_str { get; set; }
		public string previous_cursor_str { get; set; }
		public long? next_cursor { get; set; }
		public string[] ids { get; set; }
		public long? previous_cursor { get; set; }
	}

}
