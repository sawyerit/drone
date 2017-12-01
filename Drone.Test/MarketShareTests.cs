using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.API.MarketAnalysis;
using Drone.Data.Queue;
using Drone.MarketShare.Components;
using Drone.MarketShare.Datasources;
using Drone.Shared;
using Drone.QueueProcessor.Datasources;


namespace Drone.Test
{
	[TestClass]
	public class MarketShareTests
	{
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marketshare");
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Webs()
		{
			string builderName = string.Empty;

			builderName = MarketShareEngine.Instance.SiteBuilder("kitchencoach.net/");//blocked
			Assert.AreEqual("webs", builderName);
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Wix()
		{
			string builderName = string.Empty;

			builderName = MarketShareEngine.Instance.SiteBuilder("courtneytidman.co.uk");//blocked 
			Assert.AreEqual("wix", builderName);
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Weebly()
		{
			string builderName = string.Empty;

			builderName = MarketShareEngine.Instance.SiteBuilder("classfiveboats.com");
			Assert.AreEqual("weebly", builderName);
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Yola()
		{
			string builderName = string.Empty;

			builderName = MarketShareEngine.Instance.SiteBuilder("zumbahamilton.ca");
			Assert.AreEqual("yola", builderName);
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Jimdo()
		{
			string builderName = string.Empty;

			builderName = MarketShareEngine.Instance.SiteBuilder("bikewrappers.com");
			Assert.AreEqual("jimdo", builderName);
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_1and1()
		{
			string builderName = string.Empty;

            builderName = MarketShareEngine.Instance.SiteBuilder("robpaveza.net/im-going-to-be-on-channel9");
			Assert.AreEqual("1and1", builderName);
		}


		//[TestMethod]
		//public void MarketShare_SiteBuilder_Wordpress()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("biritemarket.com");
		//	Assert.AreEqual("wordpress", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_Joomla()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("okstate.edu");
		//	Assert.AreEqual("joomla", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_Drupal()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("ubuntu.com");
		//	Assert.AreEqual("drupal", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_Blogger()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("bloggerbuster.com");
		//	Assert.AreEqual("blogger", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_DotNetNuke()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("invitedtoliveunited.org");
		//	Assert.AreEqual("dot net nuke", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_YahooSiteBuilder()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("antennadeals.com");
		//	Assert.AreEqual("yahoo site builder", builderName);
		//}

		//[TestMethod]
		//public void MarketShare_SiteBuilder_WebSiteTonight()
		//{
		//	string builderName = string.Empty;

		//	builderName = MarketShareEngine.Instance.SiteBuilder("gourmetgoodiesbyalisha.com");
		//	Assert.AreEqual("website tonight", builderName);
		//}

		[TestMethod] //2048
		public void marketShare_MarketShareBuilder_GetData()
		{
			MarketShareBuilder msb = new MarketShareBuilder();
			msb.DroneDataSource = new Drone.MarketShare.Datasources.MarketShareDataSource();

			msb.GetData(msb.Context);
		}



		[TestMethod]
		public void MarketShare_ShoppingCart_Volusion()
		{
			string cartName = string.Empty;

			cartName = MarketShareEngine.Instance.ShoppingCart("saferwholesale.com");
			Assert.AreEqual("volusion", cartName);
		}

		[TestMethod]
		public void MarketShare_ShoppingCart_Shopify()
		{
			string cartName = string.Empty;

			cartName = MarketShareEngine.Instance.ShoppingCart("tattly.com");
			Assert.AreEqual("shopify", cartName);
		}

		[TestMethod]
		public void MarketShare_ShoppingCart_BigCommerce()
		{
			string cartName = string.Empty;

            cartName = MarketShareEngine.Instance.ShoppingCart("freecitylinks.com");
			Assert.AreEqual("bigcommerce", cartName);
		}



		//[TestMethod]
		//public void MarketShare_ShoppingCart_Magento()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("zumiez.com");
		//	Assert.AreEqual("magento", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_TheFind()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("tradetang.com");
		//	Assert.AreEqual("thefind upfront", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_YahooStore()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("scrapbook.com");
		//	Assert.AreEqual("yahoo store", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_Virtuemart()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("wmsoftware.com");
		//	Assert.AreEqual("virtuemart", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_ZenCart()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("therawfoodworld.com");
		//	Assert.AreEqual("zencart", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_WPEcommerce()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("eyecaredepot.com");
		//	Assert.AreEqual("wp ecommerce", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_MivaMerchant()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("helmetcity.com");
		//	Assert.AreEqual("miva merchant", cartName);
		//}

		//[TestMethod]
		//public void MarketShare_ShoppingCart_GoDaddy()
		//{
		//	string cartName = string.Empty;

		//	cartName = MarketShareEngine.Instance.ShoppingCart("kustomsmallengine.com");
		//	Assert.AreEqual("go daddy", cartName);
		//}

		[TestMethod]//4096
		public void marketShare_MarketShareCart_GetData()
		{
			//QueueManager qm = new QueueManager(@".\Private$\Drone", new string[] { "Drone.Entities.MarketShare.MarketShareDataType, Drone.Entities.MarketShare, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" });
			MarketShareCart msb = new MarketShareCart();
			msb.DroneDataSource = new Drone.MarketShare.Datasources.MarketShareDataSource();

			msb.GetData(msb.Context);
		}

		[TestMethod]
		public void MarketShare_NonHtmlContentType()
		{
			//GURULISTBUILDER.US not an html file
			string cartName = string.Empty;

			cartName = MarketShareEngine.Instance.ShoppingCart("GURULISTBUILDER.US");
			Assert.AreEqual("None", cartName);
		}


		[TestMethod]
		public void MarketShare_ShoppingCart_Any()
		{
			string cartName = string.Empty;

			cartName = MarketShareEngine.Instance.ShoppingCart("");            
		}

		[TestMethod]
		public void MarketShare_SiteBuilder_Any()
		{
			string cartName = string.Empty;

			cartName = MarketShareEngine.Instance.SiteBuilder("publichousebrewery.com");
		}
	}
}
