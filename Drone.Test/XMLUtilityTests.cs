using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.Twitter.Datasources;
using Drone.MarketShare.Components;
using Drone.MarketShare.Datasources;
using Drone.Shared;
using System.Xml;
using Drone.Crunchbase.Components;

namespace Drone.Test
{
	[TestClass]
	public class XMLUtilityTests
	{
		[TestMethod]
		public void TwitterUtility_IsEnabled()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());

			bool enabledProperty = XMLUtility.IsEnabled(t.Xml);
			Assert.IsTrue(enabledProperty);
		}

		[TestMethod]
		public void TwitterUtility_IsComponentEnabled()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());

			bool enabledProperty = XMLUtility.IsComponentEnabled(t.Xml, "twittertrend");
			Assert.IsFalse(enabledProperty);
		}

		[TestMethod]
		public void TwitterUtility_UseSinceID()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());

			bool enabledProperty = XMLUtility.UseSinceId(t.Xml, "twitter");
			Assert.IsTrue(enabledProperty);
		}

		[TestMethod]
		public void TwitterUtility_IsEnabled_BadXmlDefaultsFalse()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());

			bool enabledProperty = XMLUtility.IsEnabled(new XmlDocument());
			Assert.IsFalse(enabledProperty);
		}

		[TestMethod]
		public void TwitterUtility_GetUserID()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			long id = XMLUtility.GetUserId(t.Xml);

			Assert.IsNotNull(id);
		}

		[TestMethod]
		public void TwitterUtility_GetPageResultCounts()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			int countPerPage; int pageCount;

			XMLUtility.GetPageResultCounts(t.Xml, "twitter", out countPerPage, out pageCount, 100, 3);

			Assert.AreEqual(countPerPage, 100);
			Assert.AreEqual(pageCount, 5);
		}

		[TestMethod]
		public void TwitterUtility_GetPageResultCounts_DefaultsCorrectly()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			int countPerPage; int pageCount;

			XMLUtility.GetPageResultCounts(new XmlDocument(), "twitter", out countPerPage, out pageCount, 100, 3);

			Assert.AreEqual(countPerPage, 100);
			Assert.AreEqual(pageCount, 3);
		}

		[TestMethod]
		public void TwitterUtility_GetOAuthToken()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			Drone.API.Twitter.OAuth.OAuthTokens oat = Drone.Twitter.Utility.GetOAuthToken(t.Xml);

			Assert.IsNotNull(oat);
			Assert.IsTrue(oat.HasAccessToken);
			Assert.IsTrue(oat.HasConsumerToken);
		}

		[TestMethod]
		public void TwitterUtility_GetNextRunIntervalByNode()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			DateTime nextRun = XMLUtility.GetNextRunIntervalByNode(t.Xml, "users");

			Assert.IsNotNull(nextRun);

			t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			nextRun = XMLUtility.GetNextRunIntervalByNode(t.Xml, "trends");

			Assert.IsNotNull(nextRun);
		}

		[TestMethod]
		public void TwitterUtility_GetNextRunInterval()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			DateTime nextRun = XMLUtility.GetNextRunInterval(t.Xml);

			Assert.IsNotNull(nextRun);
		}

		[TestMethod]
		public void TwitterUtility_GetNext15MinRunTime()
		{
			Drone.Twitter.Components.Twitter t = new Twitter.Components.Twitter(new TwitterTestDataSource());
			DateTime nextRun = XMLUtility.GetNext15MinRunTime();

			Assert.IsNotNull(nextRun);
			Assert.IsTrue(nextRun.Minute % 15 == 0);
		}

		[TestMethod]
		public void Crunchbase_GetNodeText()
		{
			Crunch t = new Crunch(new MarketShareDataSource());
			XmlDocument xmlDoc = t.Xml as XmlDocument;

			string text = XMLUtility.GetTextFromAccountNode(t.Xml, "crunch/interval");
			Assert.AreEqual(text, "saturday");
		}
	}
}
