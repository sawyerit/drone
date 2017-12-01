
namespace Drone.Entities.WebAPI
{
	public class Competitor
	{
		public int ID { get; set; }
		public string Company { get; set; }
		public string Type { get; set; }
		
		public string TwitterAccount { get; set; }
		public long TwitterID { get; set; }

		public string FacebookAccount { get; set; }
		public long FacebookID { get; set; }

		public string YouTubeAccount { get; set; }
		public string YouTubeUrl { get; set; }		
	}
}
