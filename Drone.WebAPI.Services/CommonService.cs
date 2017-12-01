using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceProcess;
using Drone.Data;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
	public class CommonService : BaseService, ICommonService
	{
		public List<Competitor> GetCompetitors()
		{
			List<Competitor> compList = new List<Competitor>();
			DataTable dt = DataFactory.GetDataTableByName("GoDaddyCompetitiors");

			if (!Object.Equals(dt, null))
			{
				foreach (DataRow row in dt.Rows)
				{
					bool isFailure = false;
					Competitor c = new Competitor();
					string youTubeId = string.Empty;

					c.ID = Conversions.ConvertTo<int>(row["id"], 0, out isFailure);
					c.Company = Conversions.ConvertTo<string>(row["company"], string.Empty, out isFailure);
					c.Type = Conversions.ConvertTo<string>(row["type"], string.Empty, out isFailure);

					c.TwitterAccount = Conversions.ConvertTo<string>(row["twitterAccount"], string.Empty, out isFailure);
					c.TwitterID = Conversions.ConvertTo<long>(row["twitterID"], 0, out isFailure);

					c.FacebookAccount = Conversions.ConvertTo<string>(row["facebookAccount"], string.Empty, out isFailure);
					c.FacebookID = Conversions.ConvertTo<long>(row["facebookID"], 0, out isFailure);

					c.YouTubeAccount = Conversions.ConvertTo<string>(row["youtubeAccount"], string.Empty, out isFailure);
					c.YouTubeUrl = Conversions.ConvertTo<string>(row["youtubeURL"], string.Empty, out isFailure);

					compList.Add(c);
				}
			}

			return compList;
		}
		
		public Competitor GetCompetitor(int id)
		{
			Competitor c = new Competitor();
			DataTable dt = DataFactory.GetDataTableByName("GoDaddyCompetitiors");

			if (!Object.Equals(dt, null))
			{
				foreach (DataRow row in dt.Rows)
				{
					bool isFailure = false;
					int compID = Conversions.ConvertTo<int>(row["id"], 0, out isFailure);
					if (compID == id)
					{
						string youTubeId = string.Empty;

						c.ID = compID;
						c.Company = Conversions.ConvertTo<string>(row["company"], string.Empty, out isFailure);
						c.Type = Conversions.ConvertTo<string>(row["type"], string.Empty, out isFailure);

						c.TwitterAccount = Conversions.ConvertTo<string>(row["twitterAccount"], string.Empty, out isFailure);
						c.TwitterID = Conversions.ConvertTo<long>(row["twitterID"], 0, out isFailure);

						c.FacebookAccount = Conversions.ConvertTo<string>(row["facebookAccount"], string.Empty, out isFailure);
						c.FacebookID = Conversions.ConvertTo<long>(row["facebookID"], 0, out isFailure);

						c.YouTubeAccount = Conversions.ConvertTo<string>(row["youtubeAccount"], string.Empty, out isFailure);
						c.YouTubeUrl = Conversions.ConvertTo<string>(row["youtubeURL"], string.Empty, out isFailure);

						return c;
					}
				}
			}

			return null;
		}

		public void WriteError(string msg)
		{
			ExceptionExtensions.LogError(new ArgumentException(msg), "CommonService.WriteError", "None additional");
		}

        /// <summary>
        /// Peek at the items in the MSMQ.
        /// This is mostly used for debugging
        /// </summary>
        /// <param name="count">number of items to return</param>
        /// <returns>MSMQ messages</returns>
        public List<string> PeekQueue(int count)
        {
            return _queueManager.PeekQueue(count);
        }
    }
}