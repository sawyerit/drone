
namespace Drone.Entities.Twitter
{
	public class FollowerId
	{
		public string next_cursor_str { get; set; }
		public string previous_cursor_str { get; set; }
		public long? next_cursor { get; set; }
		public int[] ids { get; set; }
		public int? previous_cursor { get; set; }
	}

}
