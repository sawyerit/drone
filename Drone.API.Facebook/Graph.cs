using Drone.Entities.Facebook;
using System.Collections.Generic;
using System;

namespace Drone.API.Facebook
{
	public class Graph : BaseManager
	{
		public Page GetPageInfo(string accountName)
		{
			//public page data	
			return Request.Deserialize<Page>(Request.ExecuteWebRequest(string.Format("{0}/{1}", GraphUrl, accountName)));
		}

		public Page GetPageInfo(long accountID)
		{
			//public page data	
			return Request.Deserialize<Page>(Request.ExecuteWebRequest(string.Format("{0}/{1}", GraphUrl, accountID)));
		}

		public Post GetPostInfo(string accountId, string accessToken)
		{
			string url = string.Format("{2}/{0}/posts?access_token={1}", accountId, accessToken, GraphUrl);
			return Request.Deserialize<Post>(Request.ExecuteWebRequest(url));
		}

		public List<Post> GetPostsByQuery(string query)
		{
			//pipe delimit for an OR posts
			//https://graph.facebook.com/search?q=godaddy%20|%20"Go%20Daddy"&type=post&limit=100

			//place on FB (posts/user/page/event/group/place/checkin)
			//https://graph.facebook.com/search?q=godaddy&type=place&limit=100&access_token=AAACEdEose0cBALffQNzw72Iv4fzlwtj4MFpHZBT7U1MY4LXkPwoyrOZAiCkkkox1pifArZBDRHqNSgK9Ot2vrqHWNhZBbjoWmzzSTX3a3AZDZD

			//"go daddy"
			//https://graph.facebook.com/search?q=%22go%20daddy%22&type=post
			string url = string.Format("{0}/search?type=post&q={1}", GraphUrl, query);
			return Request.Deserialize<List<Post>>(Request.ExecuteWebRequest(url));
		}

		public Insight GetInsightInfo(string accountId, string accessToken)
		{
			string url = string.Format("{2}/{0}/insights?access_token={1}", accountId, accessToken, GraphUrl);

			string responseText = Request.ExecuteWebRequest(url);
			responseText = responseText.Replace("\"\"", "\"empty\"");

			return Request.Deserialize<Insight>(responseText);
		}

		public T GetFanDemographics<T>(string accountId, string accessToken, string metric) where T : new()
		{
			T returnVal = default(T);

			string url = string.Format("{3}/{0}/insights/{1}?access_token={2}", accountId, metric, accessToken, GraphUrl);

			try
			{
				returnVal = Request.Deserialize<T>(Request.ExecuteWebRequest(url).Replace("\"\"", "\"empty\""), new DemographicJavaScriptConverter());
				if (Object.Equals(null, returnVal))
				{
					returnVal = Request.Deserialize<T>(Request.ExecuteWebRequest(url).Replace("\"\"", "\"empty\""), new DemographicJavaScriptConverter());
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("deserialization"))
				{
					returnVal = Request.Deserialize<T>(Request.ExecuteWebRequest(url).Replace("\"\"", "\"empty\""), new DemographicJavaScriptConverter());
				}
				else
					throw;
			}

			return returnVal;
		}

		public static double DateTimeToUnixTimestamp(DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			TimeSpan diff = date - origin;
			return Math.Floor(diff.TotalSeconds);
		}
	}
}
