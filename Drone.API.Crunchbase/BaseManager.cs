using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.Crunchbase
{
	public class BaseManager
	{
		private XmlDocument xmlDoc = null;
		private string _apiUrl = string.Empty;
		private string _apiKey = string.Empty;
		private bool _verboseLogging;

		public string ApiUrl
		{
			get
			{
				return _apiUrl;
			}
		}

		public string ApiKey
		{
			get
			{
				return _apiKey;
			}
		}

		public bool VerboseLogging
		{
			get { return _verboseLogging; }
		}


		public BaseManager()
		{
			LoadXmlConfig();
		}

		private void LoadXmlConfig()
		{
			xmlDoc = new XmlDocument();
			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Crunchbase.xml");

			try
			{
				xmlDoc.Load(sXMLPath);

				_apiUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apiurl").Value;
				_apiKey = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("apikey").Value;
				_verboseLogging = Conversions.ConvertTo<bool>(XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("verboselogging").Value, false);
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.Crunchbase BaseManager"
																														, "BaseManager.LoadXmlConfig()"
																														, "nouser"
																														, System.Environment.MachineName
																														, "Path: " + sXMLPath));
			}
		}

	}
}
