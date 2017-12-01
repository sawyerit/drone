
namespace Drone.Entities.Facebook
{
	public class Page
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Link { get; set; }
		public string Category { get; set; }
		public string Website { get; set; }
		public string Username { get; set; }
		public int Talking_About_Count { get; set; }
		public int Likes { get; set; }

		public Page() { }
	}
}
