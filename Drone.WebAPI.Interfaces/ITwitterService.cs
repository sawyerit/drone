using System.Collections.Generic;
using Drone.Entities.Twitter.v11;

namespace Drone.WebAPI.Interfaces
{
	public interface ITwitterService
	{
		KeywordStatus CreateMentions(KeywordStatus value);
		List<User> CreateUsers(List<User> value);
		
		List<Keyword> GetKeywords();
		Status GetMention(int id);
		List<KeywordStatus> GetAllMentions();
		List<KeywordStatus> GetPaged(int page, int p);		
	}
}
