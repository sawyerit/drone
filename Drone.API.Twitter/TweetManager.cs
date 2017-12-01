using System;
using System.Collections.Generic;
using Drone.Entities.Twitter;
using Drone.API.Twitter.OAuth;
using Drone.Entities.Twitter.v11;

namespace Drone.API.Twitter
{
	public class TweetManager : BaseManager
	{
		/// <summary>
		/// Returns all @mentions and @replies to the authenticated user
		/// Not used
		/// </summary>
		/// <param name="countPerPage">results per page, max value 200</param>
		/// <param name="pagesToReturn">number of pages to return</param>
		/// <returns>As many results as it can, depending on data and service limit restrictions</returns>
		public List<Mention> GetMentionsAndReplies(int countPerPage, int pagesToReturn, OAuthTokens tokens)
		{
			List<Mention> _twitterMentionData = new List<Mention>();
			countPerPage = countPerPage > 200 ? 200 : countPerPage;

			for (int i = 1; i <= pagesToReturn; i++)
			{
				string requestText = string.Format("{2}/statuses/mentions.json?include_entities=false&include_rts=false&count={0}&page={1}", countPerPage, i, ApiUrl);
				_twitterMentionData.AddRange(Request.Deserialize<List<Mention>>(Request.ExecuteAuthenticatedWebRequest(requestText, tokens)));
			}

			return _twitterMentionData;
		}

		/// <summary>
		/// Returns all tweets with the word GoDaddy in them.  Including @GoDaddy, GoDaddy.com etc.  
		/// Max speed before throttling seems to be 15pages, once a minute. (since we have 6 queries) 
		/// </summary>
		/// <param name="countPerRequest">results per page, up to 100</param>
		/// <param name="pagesToReturn">pages to return up to 1500 results (countPerPage * pagesToReturn)</param>
		/// <param name="paramString">query param string that is sent to twitter</param>
		/// <returns></returns>
		public List<Status> GetTweetsByQuery(int countPerRequest, string paramString, OAuthTokens tokens)
		{
			List<Status> _totalTwitterMentionResultData = new List<Status>();
			countPerRequest = countPerRequest > 100 ? 100 : countPerRequest;

			string requestText = string.Format("{2}/tweets.json?q={0}&count={1}&result_type=recent", paramString, countPerRequest, SearchUrl);

			RootStatus _totalTwitterMentionData = Request.Deserialize<RootStatus>(Request.ExecuteAuthenticatedWebRequest(requestText, tokens));

			if (!Object.Equals(_totalTwitterMentionData.statuses, null))
			{
				_totalTwitterMentionResultData.AddRange(_totalTwitterMentionData.statuses);
			}

			//TODO: implement max_id to get past tweets.
			//if (_totalTwitterMentionData.statuses.Count >= countPerRequest && !paramString.Contains("since_id"))
			//{
			//	return GetTweetsByQuery(countPerRequest, paramString + "&max_id=" + _totalTwitterMentionData.statuses[_totalTwitterMentionData.statuses.Count -1 ].id, tokens);
			//}

			return _totalTwitterMentionResultData;
		}

		/// <summary>
		/// Returns all direct replies to the authenticated user
		/// Not used
		/// </summary>
		/// <param name="countPerPage">results per page, max value 200</param>
		/// <param name="pagesToReturn">number of pages to return</param>
		/// <returns>As many results as it can, depending on data and service limit restrictions</returns>
		public List<DirectMessage> GetDirectMessages(int countPerPage, int pagesToReturn, OAuthTokens tokens)
		{
			List<DirectMessage> _twitterDirectMessageData = new List<DirectMessage>();
			countPerPage = countPerPage > 200 ? 200 : countPerPage;

			for (int i = 1; i <= pagesToReturn; i++)
			{
				string requestText = string.Format("{2}/direct_messages.json?include_entities=false&count={0}&page={1}", countPerPage, i, ApiUrl);
				_twitterDirectMessageData.AddRange(Request.Deserialize<List<DirectMessage>>(Request.ExecuteAuthenticatedWebRequest(requestText, tokens)));
			}

			return _twitterDirectMessageData;
		}
	}
}
