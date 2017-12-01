using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Drone.API.Twitter;
using Drone.API.Twitter.OAuth;
using Drone.Entities.Twitter;
using Drone.Entities.Twitter.v11;
using Drone.Shared;
using Drone.Twitter.Components;
using Drone.Twitter.Datasources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drone.Test
{
	[TestClass]
	public class TwitterTests
	{
		#region Component Tests

		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "socialmedia");
		}

		[TestMethod]
		public void Twitter_GetData()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterDataSource());
			t.GetData(t.Context);
			TwitterDataComponent tdc = t.DroneDataComponent as TwitterDataComponent;

			Assert.AreEqual(Drone.Twitter.Components.Twitter.ComponentTypeName, "TwitterComponent");
			//Assert.IsNotNull(tdc.AllTwitterMentions);
		}

		[TestMethod]
		public void Twitter_GetAvailablePlaces()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			TwitterDataComponent tdc = t.DroneDataComponent as TwitterDataComponent;
			tdc.TwitterAvailablePlaces = Drone.Twitter.Components.Twitter.GetAvailablePlaces();

			foreach (var item in tdc.TwitterAvailablePlaces.OrderBy(x => x.country))
			{
				Debug.WriteLine(string.Format("Country:{0}.  Name:{1}.  WoeID:{3}  Type:{2}", item.country, item.name, item.placeType.name, item.woeid));
			}

			Assert.IsNotNull(tdc.TwitterAvailablePlaces);
		}

		[TestMethod]
		[Ignore]
		public void Twitter_GetTrendsForAllPlaces()
		{
			TwitterTrend tt = new TwitterTrend(new TwitterTestDataSource());
			tt.GetData(tt.Context);
			TwitterDataComponent tdc = tt.DroneDataComponent as TwitterDataComponent;

			Assert.IsNotNull(tdc.TrendRootList);
			Assert.AreNotEqual(0, tdc.TrendRootList.Count());
		}

		[TestMethod]
		[Ignore]
		public void Twitter_GetData_RateLimitTest()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			while (true)
			{
				try
				{
					t.GetData(t.Context);
				}
				catch (System.Exception)
				{ }
				Thread.Sleep(60000); //one minute seems to be the fastest w/out hitting rate limits
			}
		}

		[TestMethod]
		public void TwitterUser_GetData()
		{
			TwitterUser t = new TwitterUser(new TwitterUserDataSource());
			t.GetData(t.Context);
			TwitterDataComponent tdc = t.DroneDataComponent as TwitterDataComponent;

			Assert.IsNull(tdc.TwitterUserList); //first time through will not run... don't need to collect data just set the next run interval
			Assert.IsNotNull(t.Context.NextRun);

			t.GetData(t.Context);
			Assert.IsNotNull(tdc.TwitterUserList);
			Assert.IsTrue(tdc.TwitterUserList[0].followers_count > 0);
		}

		[TestMethod]
		public void TwitterFollower_GetData()
		{
			TwitterFollower t = new TwitterFollower(new TwitterFollowerDataSource());
			t.GetData(t.Context);
			TwitterDataComponent tdc = t.DroneDataComponent as TwitterDataComponent;
		}

		[TestMethod]
		public void TwitterUser_GetALlFollowers()
		{
			Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			OAuthTokens oat = Drone.Twitter.Utility.GetOAuthToken(t.Xml);

			UserManager um = new UserManager();
			um.GetAllFollowers(oat);
		}

		[TestMethod]
		public void TwitterUser_GetFollowerCountsFromCSV()
		{
			Utility.ComponentBaseFolder = AppDomain.CurrentDomain.BaseDirectory;

			Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			OAuthTokens oat = Drone.Twitter.Utility.GetOAuthToken(t.Xml);

			UserManager um = new UserManager();
			um.GetFollowerCounts(oat);
		}


		#endregion

		#region Sentiment POC testing

		[TestMethod]
		[Ignore]
		public void Twitter_ScoreSentiment()
		{
			//Sentiment API from ViralHeat.com
			string query = "http://www.viralheat.com/api/sentiment/review.json?text=Is this a good thing?&api_key=2WrsZcqYmrKNGtKBwtc";

			HttpWebResponse response = null;
			string responseText = string.Empty;

			WebRequest request = WebRequest.Create(query);
			request.ContentType = "application/json; charset=utf-8";
			response = (HttpWebResponse)request.GetResponse();

			if (!Object.Equals(response, null))
				responseText = BuildTextFromResponse(response);
		}

		private string BuildTextFromResponse(WebResponse response)
		{
			string responseText;
			using (var streamReader = new StreamReader(response.GetResponseStream()))
				responseText = streamReader.ReadToEnd();

			return responseText;
		}

		#endregion

		[TestMethod]
		[Ignore]
		public void UrlEncodeTest()
		{
			//HtmlDecode doesn't work, but EscapeURI does
			string toDecode = "%23GoDaddyDomains test decode";
			string decoded = string.Empty;

			decoded = System.Net.WebUtility.HtmlDecode(toDecode);

			Assert.IsTrue(decoded.Contains("%23"));

			decoded = System.Uri.UnescapeDataString(toDecode);

			Assert.IsFalse(decoded.Contains("%23"));
		}

	}
}

