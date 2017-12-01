using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Drone.API.Twitter;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Shared;
using Drone.Twitter.Datasources;

namespace Drone.Twitter.Components
{
	[Export(typeof(IDroneComponent))]
	public class Twitter : BaseComponent<TwitterComponent>
	{
		#region public properties

		internal TwitterDataComponent _dataComponent { get; set; }

		#endregion

		#region constructors

		[ImportingConstructor]
		public Twitter()
			: base()
		{
			DroneDataSource = new TwitterDataSource();
			DroneDataComponent = new TwitterDataComponent();
			Context = new TwitterContext { Status = "waiting", TimeOfStatus = DateTime.Now, DurationPreviousStatus = TimeSpan.FromSeconds(0) };
		}

		public Twitter(IDroneDataSource datasource)
			: base(datasource)
		{
			DroneDataSource = datasource;
			DroneDataComponent = new TwitterDataComponent();
			Context = new TwitterContext { Status = "waiting", TimeOfStatus = DateTime.Now, DurationPreviousStatus = TimeSpan.FromSeconds(0) };
		}

		#endregion

		/// <summary>
		/// Main method that gathers data
		/// </summary>
		/// <param name="context">IDroneContext</param>
		public override void GetData(object context)
		{
			try
			{
				TwitterContext cont = context as TwitterContext;
				if (!Object.Equals(cont, null))
				{
					SetContextStatus("processing", cont);

					//lock the thread and write to context
					lock (cont) cont.NextRun = XMLUtility.GetNextRunInterval(Xml);

					//do work
					if (XMLUtility.IsEnabled(Xml))
					{
						WriteToUsageLogFile("Twitter.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Started GetAllMentionsByQuery");

						GetAllMentionsByQuery(cont);

						WriteToUsageLogFile("Twitter.GetData | Thread:" + Thread.CurrentThread.ManagedThreadId.ToString(), "Completed GetAllMentionsByQuery");
					}

					//lock the thread and write to context
					SetContextStatus("waiting", cont);
				}

				RaiseProcessingCompletedEvent(new EventArgs());
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.Twitter.Components.Twitter.GetData()");
			}
		}


		#region internal methods

		/// <summary>
		/// Get all mentions and replies @GoDaddy
		/// Not used		
		/// </summary>
		internal List<Mention> GetMentionsReplies()
		{
			int countPerPage; int pageCount;
			XMLUtility.GetPageResultCounts(Xml, "mentionsreplies", out countPerPage, out pageCount, 200, 1);
			TweetManager tm = new TweetManager();

			return tm.GetMentionsAndReplies(countPerPage, pageCount, Utility.GetOAuthToken(Xml));
		}

		/// <summary>
		/// Get all direct messages
		/// Not used
		/// </summary>
		internal List<DirectMessage> GetDirectMessages()
		{
			int countPerPage; int pageCount;
			XMLUtility.GetPageResultCounts(Xml, "mentionsreplies", out countPerPage, out pageCount, 200, 1);
			TweetManager tm = new TweetManager();

			return tm.GetDirectMessages(countPerPage, pageCount, Utility.GetOAuthToken(Xml));
		}

		/// <summary>
		/// Get all tweets with "GoDaddy" in them. 
		/// 100 per page, up to 15 pages if we loop and make seperate calls for each page
		/// </summary>
		internal Dictionary<int, List<Status>> GetAllMentionsByQuery(TwitterContext cont)
		{
			Dictionary<int, List<Status>> allTwitterMentions = new Dictionary<int, List<Status>>();

			if (XMLUtility.IsComponentEnabled(Xml, ProcessorName))
			{
				int countPerPage; int pageCount;
				TwitterDataSource data = new TwitterDataSource();
				TweetManager tm = new TweetManager();

				XMLUtility.GetPageResultCounts(Xml, ProcessorName, out countPerPage, out pageCount, 100, 3);
				List<Keyword> queries = data.GetTweetQueries();
				bool useSinceId = XMLUtility.UseSinceId(Xml, ProcessorName);

				//create backup of current keys in case of failure at db level
				lock (cont) cont.prevRunLatestTweetIDs = cont.LatestTweetIDs.ToDictionary(entry => entry.Key, entry => entry.Value);

				foreach (Keyword item in queries)
				{
					try
					{
						long sinceId = 0;
						if (useSinceId)
						{
							//get the last recorded id for this query and use it
							if (!Object.Equals(cont, null))
							{
								lock (cont)
								{
									if (cont.LatestTweetIDs.ContainsKey(item.KeywordId))
										cont.LatestTweetIDs.TryGetValue(item.KeywordId, out sinceId);
								}
							}
						}

						//call the mention object in the API wrapper
						TwitterDataComponent _dataComponent = DroneDataComponent as TwitterDataComponent;
						KeywordStatus ks = new KeywordStatus();
						ks.KeywordId = item.KeywordId;
						
						ks.StatusList = tm.GetTweetsByQuery(countPerPage, item.KeywordValue + (sinceId > 0 ? "&since_id=" + sinceId : string.Empty), Utility.GetOAuthToken(Xml));
						allTwitterMentions.Add(ks.KeywordId, ks.StatusList);

						_dataComponent.KeywordStatus = ks;
						DroneDataSource.Process(_dataComponent);

						//if there was a failure saving to the db, reset the since id to gather and try again
						if (_dataComponent.SaveFailure)
							lock (cont) cont.LatestTweetIDs = cont.prevRunLatestTweetIDs.ToDictionary(entry => entry.Key, entry => entry.Value);

						//get the last id for this query and store it
						if (!Object.Equals(cont, null) && allTwitterMentions.ContainsKey(item.KeywordId) && allTwitterMentions[item.KeywordId].Count > 0)
						{
							long latestID;
							long.TryParse(allTwitterMentions[item.KeywordId][0].id.ToString(), out latestID);

							lock (cont)
							{
								if (cont.LatestTweetIDs.ContainsKey(item.KeywordId))
									cont.LatestTweetIDs[item.KeywordId] = latestID;
								else
									cont.LatestTweetIDs.Add(item.KeywordId, latestID);
							}
						}
					}
					catch (Exception e)
					{

						ExceptionExtensions.LogError(e, "Twitter.GetAllMentionsByQuery()", "Keyword name: " + item.KeywordName);
					}
				}
			}
			return allTwitterMentions;
		}

		//Not used
		internal static List<Place> GetAvailablePlaces()
		{
			return new PlaceManager().GetAvailablePlaces();
		}

		#endregion

		#region WebService methods

		/// <summary>
		/// Public exposed method for calling from a webservice. 
		/// Querystring will be "q=[paramstring]"
		/// </summary>
		/// <param name="countPerPage"></param>
		/// <param name="keywordID">keyword id from rptTwitterKeywordLookup (1 through 6)</param>
		/// <param name="pagesToReturn"></param>
		/// <param name="paramString">any valid parameters except for count and pageCount.</param>
		public static Dictionary<int, List<Status>> GetAllMentionsByQuery(int keywordID, int countPerPage, int pagesToReturn, string paramString)
		{
			Dictionary<int, List<Status>> allTwitterMentions = new Dictionary<int, List<Status>>();
			TweetManager tm = new TweetManager();
			Twitter t = new Twitter();
			allTwitterMentions.Add(keywordID, tm.GetTweetsByQuery(countPerPage, paramString, Utility.GetOAuthToken(t.Xml)));

			return allTwitterMentions;
		}

		#endregion

	}
}
