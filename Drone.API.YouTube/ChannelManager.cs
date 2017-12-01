using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Google.GData.Client;
using Google.GData.YouTube;
using Google.YouTube;
using Drone.Entities.YouTube;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.YouTube
{
	public class ChannelManager
	{
		private Channel _channel = new Channel();
		private string _appName = string.Empty;
		private string _devKey = string.Empty;
		private string _user = string.Empty;
		private int _userID = 0;
		private string _apiUploadsUrl = string.Empty;
		private string _apiChannelUrl = string.Empty;
		private string _apiVideosUrl = string.Empty;

		public ChannelManager(KeyValuePair<int, string> account, string appName, string devKey)
		{
			_appName = appName;
			_devKey = devKey;
			_user = account.Value;
			_userID = account.Key;
			LoadXmlConfig();
		}

		public ChannelManager()
		{
			LoadXmlConfig();
		}

		public Channel GetUserChannel()
		{
			BuildChannel(String.Format(_apiChannelUrl, _user));
			AddVideoFeed(new YouTubeQuery(String.Format(_apiUploadsUrl, _user)));

			return _channel;
		}

		private void BuildChannel(string channelQuery)
		{
			//Get profile and create channel object
			try
			{
				YouTubeService service = new YouTubeService(_appName, _devKey);
				if (!Object.Equals(service, null))
				{
					ProfileEntry profile = (ProfileEntry)service.Get(channelQuery);

					if (!Object.Equals(profile, null))
						_channel = new Channel(profile, _user, _userID);
				}
			}
			catch (GDataRequestException gdre)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleException(gdre.Message, "Query: " + channelQuery, Utility.ApplicationName, LogTypeEnum.Error, LogActionEnum.LogAndEmail, "ChannelManager.BuildChannel()", "Drone.API.YouTube Exception", string.Empty, string.Empty, System.Environment.MachineName);
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log, LogTypeEnum.Error, "Drone.API.YouTube Exception", "ChannelManager.BuildChannel()", "nouser", System.Environment.MachineName, "Query: " + channelQuery));
			}
		}

		private void AddVideoFeed(YouTubeQuery q)
		{
			try
			{
				YouTubeRequest request = GetRequest();
				Feed<Video> feed = request.Get<Video>(q);

                feed.Maximum = 500;//11_5_13 ss: limited to 500 now??

				if (!Object.Equals(feed, null) && !Object.Equals(feed.Entries, null))
					foreach (Video vid in feed.Entries)
						_channel.Feed.Add(new ChannelVideo(vid));
			}
			catch (GDataRequestException gdre)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleException(gdre.Message, "Query: " + q.Query, Utility.ApplicationName, LogTypeEnum.Error, LogActionEnum.LogAndEmail, "ChannelManager.GetVideos()", "Drone.API.YouTube Exception", string.Empty, string.Empty, System.Environment.MachineName);
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log, LogTypeEnum.Error, "Drone.API.YouTube Exception", "ChannelManager.GetVideos()", "nouser", System.Environment.MachineName, "Query: " + q.Query));
			}

		}

		public Channel GetSquatterVideos(Collection<string> videoIds)
		{
			Channel squatterChannel = new Channel();
			squatterChannel.ID = 0;
			YouTubeRequest request = GetRequest();

			foreach (string id in videoIds)
			{
				Video vid = request.Retrieve<Video>(new Uri(string.Format(_apiVideosUrl, id)));
				squatterChannel.Feed.Add(new ChannelVideo(vid));
			}

			return squatterChannel;
		}

		#region private methods (YouTube .NET Library calls)

		private YouTubeRequest GetRequest()
		{
			YouTubeRequestSettings settings = new YouTubeRequestSettings(_appName, _devKey);
			settings.AutoPaging = true;
			YouTubeRequest request = new YouTubeRequest(settings);

			return request;
		}

		private void LoadXmlConfig()
		{
			XmlDocument xmlDoc = new XmlDocument();
			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_YouTube.xml");

			try
			{
				xmlDoc.Load(sXMLPath);

				_apiUploadsUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apiuploadsurl").Value;
				_apiChannelUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apichannelurl").Value;
				_apiVideosUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apivideosurl").Value;
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.YouTube Exception"
																														, "ChannelManager.LoadXmlConfig()"
																														, "nouser"
																														, System.Environment.MachineName
																														, "Path: " + sXMLPath));
			}
		}

		#endregion
	}
}


//not used
//public void GetTopFeed(int numToGet, string user)
//{
//  string responseText = Request.ExecuteAnonymousWebRequest(String.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads?orderby=viewCount&max-results={1}", user, numToGet));
//  ParseAllVideoXml(responseText);
//}
//not used
//public void GetNewestFeed(int numToGet, string user)
//{
//  string responseText = Request.ExecuteAnonymousWebRequest(String.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads?orderby=published&max-results={1}", user, numToGet));
//  ParseAllVideoXml(responseText);
//}

