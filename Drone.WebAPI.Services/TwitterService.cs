using System;
using System.Collections.Generic;
using System.Data;
using Drone.Data;
using Drone.Entities.Twitter.v11;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
	public class TwitterService : BaseService, ITwitterService
	{
		public List<Keyword> GetKeywords()
		{
			List<Keyword> keywordList = new List<Keyword>();
			DataTable dt = DataFactory.GetDataTableByName("TwitterKeywordLookup");

			if (!Object.Equals(dt, null))
			{
				foreach (DataRow row in dt.Rows)
				{
					keywordList.Add(new Keyword(row));
				}
			}

			return keywordList;
		}

		public Status GetMention(int id)
		{
			return new Status { id = id, user = new User { name = "TwitterUser" }, text = "Tweet text" };
		}

		public List<KeywordStatus> GetAllMentions()
		{
			return FakeMentionsList(100);
		}

		public List<KeywordStatus> GetPaged(int page, int countPerPage)
		{
			return FakeMentionsList(10);
		}

		public KeywordStatus CreateMentions(KeywordStatus value)
		{
			try
			{
				_queueManager.AddToQueue(Utility.SerializeToXMLString<KeywordStatus>(value), "Twitter KeywordStatus" + value.KeywordId);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "TwitterService.CreateMentions");
			}

			return value;
		}

		public List<User> CreateUsers(List<User> value)
		{
			try
			{
				_queueManager.AddToQueue(Utility.SerializeToXMLString<List<User>>(value), "Twitter List<User>");
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "TwitterService.CreateUsers");
			}

			return value;
		}




		//remove when real logic is implemented
		private List<KeywordStatus> FakeMentionsList(int count)
		{
			List<KeywordStatus> kl = new List<KeywordStatus>();
			List<Status> sl = new List<Status>();
			
			for (int i = 0; i < count; i++)
			{
				sl.Add(new Status { id = i, text = "tweet" + i.ToString(), user = new User { name = "GoDaddy" } });
			}

			KeywordStatus ks = new KeywordStatus { KeywordId = 2, StatusList = sl };
			kl.Add(ks);
			ks = new KeywordStatus { KeywordId = 3, StatusList = sl };
			kl.Add(ks);

			return kl;
		}
	}
}