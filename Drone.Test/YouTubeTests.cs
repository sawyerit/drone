using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.API.YouTube;
using Drone.Entities.YouTube;
using Drone.Data.Queue;
using Drone.Shared;
using Drone.YouTube.Components;
using Drone.YouTube.Datasources;

namespace Drone.Test
{
	[TestClass]
	public class YouTubeTests
	{
		ChannelManager _cManager = null;

		[TestInitialize]
		public void ChannelManager_Initialization()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "socialmedia");

			_cManager = new ChannelManager(new KeyValuePair<int, string> (1, "GoDaddy"), "Drone Processor", "AI39si7H3JsgcnrDNQ_-_NVMklAztUBMREtgH3pdiIb0iX9ASor__Nw5q2tT-0V1H8gnnVZyFsrPQpUmBcdAS6HWswa4UNaUyw");
		}

		[TestMethod]
		public void ChannelManager_ConstructsManager()
		{
			Assert.IsNotNull(_cManager);
		}

		[TestMethod]
		public void ChannelManager_GetsAllVideos_GoDaddy()
		{
			Channel c = _cManager.GetUserChannel();

			List<ChannelVideo> allVideos = c.Feed;

			Assert.IsTrue(allVideos.Count > 0);

			Assert.AreEqual(allVideos.MostPopular().Count, 5);
			Assert.AreEqual(allVideos.MostPopular(3).Count, 3);

			Assert.AreEqual(allVideos.MostRecent().Count, 5);
			Assert.AreEqual(allVideos.MostRecent(3).Count, 3);

			Assert.IsTrue(c.TotalComments > 0);
			Assert.IsTrue(c.TotalDislikes > 0);
			Assert.IsTrue(c.TotalLikes > 0);
			Assert.IsTrue(c.TotalSubscribers > 0);
			Assert.IsTrue(c.TotalVideoViews > 0);
		}

		/// <summary>
		/// Full test with DB calls
		/// </summary>
		[TestMethod]
		public void YouTube_GetData()
		{
			YouTube.Components.YouTube t = new YouTube.Components.YouTube();
			t.DroneDataSource = new YouTubeDataSource();
			t.GetData(t.Context);

			Assert.AreEqual(YouTube.Components.YouTube.ComponentTypeName, "YouTubeComponent");
		}

		[TestMethod]
		[Ignore]
		public void LinqToXML()
		{
			XmlDocument xmlDoc = new XmlDocument();

			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder, "test_video.xml");
			string xmlString = String.Empty;
			xmlDoc.Load(sXMLPath);
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = null;
			try
			{
				xw = new XmlTextWriter(sw);
			}
			catch
			{
				if (sw != null) sw.Dispose();
			}

			using (xw)
			{
				xmlDoc.WriteTo(xw);
				xmlString = sw.ToString();
			}

			XNamespace ns = "http://www.w3.org/2005/Atom";
			var descendants = from i in XDocument.Parse(xmlString).Descendants(ns + "entry")
												select i;

			foreach (XElement entry in descendants)
			{
				string id = entry.Element(ns + "id").Value;
			}
		}
	}
}
