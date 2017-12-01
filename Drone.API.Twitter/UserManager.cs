using Drone.Entities.Twitter.v11;
using System.Collections.Generic;
using Drone.API.Twitter.OAuth;
using System.Threading;
using System.Linq;
using Drone.Shared;

namespace Drone.API.Twitter
{
	/// <summary>
	/// Returns a twitter user object for the passed in id
	/// </summary>
	public class UserManager : BaseManager
	{
		public User GetTwitterUserInfo(long twitterId, OAuthTokens tokens)
		{
			string responseText = Request.ExecuteAuthenticatedWebRequest(string.Format("{0}/users/show.json?user_id={1}", ApiUrl, twitterId), tokens);
			User _twitterUserObject = Request.Deserialize<User>(responseText);

			return _twitterUserObject;
		}

		/// <summary>
		/// Gets all followers ids and then inflates follower objects for each follower
	/// </summary>
		/// <param name="tokens"></param>
		/// <returns></returns>
		public List<User> GetAllFollowers(OAuthTokens tokens)
		{
			long cursor = -1;
			List<string> allIds = new List<string>();
			string responseText = string.Empty;
			List<User> twitterFollowers = new List<User>();

			while (cursor != 0)
			{
                responseText = Request.ExecuteAuthenticatedWebRequest(string.Format("{0}/followers/ids.json?user_id=14949454&include_entities=true&cursor=" + cursor, ApiUrl), tokens);

				FollowerId twitterFollowerIds = Request.Deserialize<FollowerId>(responseText);
				if (!object.Equals(null, twitterFollowerIds.ids))
				{
					allIds.AddRange(twitterFollowerIds.ids);
					cursor = twitterFollowerIds.next_cursor.Value;
				}
				Thread.Sleep(60000);//sleep to avoid rate limit
			}

			int curStep = 0;

			while (curStep < allIds.Count)
			{
				responseText = Request.ExecuteAuthenticatedWebRequest(string.Format("{0}/users/lookup.json?include_entities=true&user_id={1}", ApiUrl, string.Join(",", allIds.Skip(curStep).Take(100))), tokens);
				twitterFollowers.AddRange(Request.Deserialize<List<User>>(responseText));
				curStep = twitterFollowers.Count;
				Thread.Sleep(6000); //sleep to avoid rate limit
			}

			return twitterFollowers;
		}

		public List<User> GetFollowerCounts(OAuthTokens tokens)
		{
			List<string> hundreditems = new List<string>();
			string responseText = string.Empty;
			List<User> twitterFollowers = new List<User>();

			List<string> tempLines;
			while ((tempLines = Utility.ReadLinesFromFile("Xml/HandlesForCounts.csv", 75, true)).Count() > 0)
			{
				foreach (string lineItem in tempLines)
				{
					string[] twitterLine = lineItem.Split(',');
					if (twitterLine.Length > 1)
					{
						hundreditems.Add(CleanName(twitterLine[1]));
					}
				}

				responseText = Request.ExecuteAuthenticatedWebRequestPost(string.Format("{0}/users/lookup.json?include_entities=false&screen_name={1}", ApiUrl, string.Join(",", hundreditems)), tokens);
				twitterFollowers.AddRange(Request.Deserialize<List<User>>(responseText));

				Utility.WriteToLogFile("Xml/HandlesWithCounts.csv", GetFollowersArray(twitterFollowers));

				twitterFollowers.Clear();
				hundreditems.Clear();

				Thread.Sleep(5000); //sleep to avoid rate limit					

			}



			//var query = from usr in twitterFollowers
			//						select new  { ScreenName = usr.screen_name };

			//string csv = string.Join(",", query.ToList());
			//return query.ToList();

			return twitterFollowers;
		}

		private string[] GetFollowersArray(List<User> twitterFollowers)
		{
			List<string> usrList = new List<string>();
			foreach (User usr in twitterFollowers)
			{
				usrList.Add(string.Format("{0},{1},{2},{3}", usr.id_str, usr.screen_name, usr.followers_count ?? 0, usr.statuses_count ?? 0));
			}

			return usrList.ToArray();
		}

		private string CleanName(string p)
		{
			string cleanP = p.Trim();

			if (cleanP.Contains("/statuses") || p.Contains("/status"))
				cleanP = cleanP.Substring(0, p.IndexOf("/"));

			return cleanP.Replace("/", "");
		}

	}
}
