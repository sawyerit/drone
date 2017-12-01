using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.Twitter
{
	public class BaseManager
	{
		private XmlDocument xmlDoc = null;
		private string _apiUrl = string.Empty;
		private string _searchUrl = string.Empty;

		public string ApiUrl
		{
			get
			{
				return _apiUrl;
			}
		}
		public string SearchUrl
		{
			get
			{
				return _searchUrl;
			}
		}
		
		public BaseManager()
		{
			LoadXmlConfig();
		}

		private void LoadXmlConfig()
		{
			xmlDoc = new XmlDocument();
			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Twitter.xml");

			try
			{
				xmlDoc.Load(sXMLPath);

				_apiUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apiurl").Value;
				_searchUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("searchurl").Value;
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.Twitter BaseManager"
																														, "baseManager.LoadXmlConfig()"
																														, "nouser"
																														, System.Environment.MachineName
																														, "Path: " + sXMLPath));
			}
		}

	}
}
