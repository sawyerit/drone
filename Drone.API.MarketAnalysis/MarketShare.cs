using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SocialMedia.API.MarketAnalysis
{
	/// <summary>
	/// Main class to expose values to consumers
	/// </summary>
	public class MarketShare
	{
		//MarketShareProcessor _processor;

		private XmlDocument _siteBuilderRulesXML;
		private XmlDocument _shoppingCartRulesXML;

		public string SiteBuilder(string domain)
		{
			string siteBuilder = "None";
			if (!String.IsNullOrWhiteSpace(domain))
			{
				//siteBuilder = MarketShareProcessor.Process(_siteBuilderRulesXML, domain);
			}
			return siteBuilder;
		}

		public string ShoppingCart(string domain)
		{
			string shoppingCart = "None";
			if (!String.IsNullOrWhiteSpace(domain))
			{
				//shoppingCart = MarketShareProcessor.Process(_shoppingCartRulesXML, domain);
			}
			return shoppingCart;
		}

		//public MarketShare(string domain)
		//{
		//	_processor = new MarketShareProcessor(domain);
		//}

		public MarketShare()
		{
			try
			{
				////load XML
				//XmlDocument xmlDoc = new XmlDocument();
				//xmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SiteBuilderRules.xml"));
				//_siteBuilderRulesXML = xmlDoc;

				//XmlDocument xmlDoc2 = new XmlDocument();
				//xmlDoc2.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShoppingCartRules.xml"));
				//_shoppingCartRulesXML = xmlDoc2;


				//load processors

				//string processorType = navigator.GetAttribute("rule type", String.Empty); //get from xml
				//if (!String.IsNullOrWhiteSpace(processorType))
				//{
				//	Assembly assembly = Assembly.GetExecutingAssembly();
				//	Type type = assembly.GetType(String.Format("{0}.{1}", typeof(MainEngine).Namespace, processor));
				//	IProcessorEngine engine = Activator.CreateInstance(type) as IProcessorEngine;
				//	data = engine.Process(node, competitorId, data);
				//}

			}
			catch (Exception e)
			{

				throw;
			}
		}
	}
}
