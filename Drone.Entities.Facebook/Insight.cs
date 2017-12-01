namespace Drone.Entities.Facebook
{
	public class Insight
	{
		public Item[] data { get; set; }
	}

	public class Item
	{
		public string id { get; set; }
		public string name { get; set; }
		public string period { get; set; }
		public Value[] values { get; set; }
		public string description { get; set; }
	}

	public class Value
	{
		public object value { get; set; }
		public string end_time { get; set; }
	}
}
