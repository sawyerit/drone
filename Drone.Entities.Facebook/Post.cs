
namespace Drone.Entities.Facebook
{
	public class PostFrom
	{
		public string name { get; set; }
		public string category { get; set; }
		public string id { get; set; }
	}

	public class Action
	{
		public string name { get; set; }
		public string link { get; set; }
	}

	public class Privacy
	{
		public string description { get; set; }
		public string value { get; set; }
	}

	public class LikeData
	{
		public string name { get; set; }
		public string id { get; set; }
		public string category { get; set; }
	}

	public class Likes
	{
		public LikeData[] data { get; set; }
		public int count { get; set; }
	}

	public class CommentFrom
	{
		public string name { get; set; }
		public string id { get; set; }
		public string category { get; set; }
	}

	public class CommentData
	{
		public string id { get; set; }
		public CommentFrom from { get; set; }
		public string message { get; set; }
		public string created_time { get; set; }
		public int? likes { get; set; }
	}

	public class Comments
	{
		public CommentData[] data { get; set; }
		public int count { get; set; }
	}

	public class ToData
	{
		public string name { get; set; }
		public string category { get; set; }
		public string id { get; set; }
	}

	public class To
	{
		public ToData[] data { get; set; }
	}

	public class PostData
	{
		public string id { get; set; }
		public PostFrom from { get; set; }
		public string message { get; set; }
		public string picture { get; set; }
		public string link { get; set; }
		public string name { get; set; }
		public string icon { get; set; }
		public Action[] actions { get; set; }
		public Privacy privacy { get; set; }
		public string type { get; set; }
		public string object_id { get; set; }
		public string created_time { get; set; }
		public string updated_time { get; set; }
		public Likes likes { get; set; }
		public Comments comments { get; set; }
		public To to { get; set; }
		public string source { get; set; }
		public string caption { get; set; }
		public string description { get; set; }
	}

	public class Post
	{
		public PostData[] data { get; set; }
	}
}
