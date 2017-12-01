using System;
using System.Collections.Generic;
using System.Data;
using Drone.Shared;

namespace Drone.Entities.Twitter
{
	public class Keyword
	{
		public int KeywordId { get; set; }
		public string KeywordName { get; set; }
		public string KeywordValue { get; set; }

		public Keyword(DataRow dr)
		{
			if (!Object.Equals(dr, null))
			{
				KeywordId = dr["keywordID"].ToString().ConvertStringToInt(0);
				KeywordName = dr["keywordName"].ToString();
				KeywordValue = dr["keyword"].ToString();
			}
		}

		public Keyword() { }
	}

	public class KeywordStatus
	{
		public int KeywordId { get; set; }
		public List<Status> StatusList { get; set; }
	}
}
