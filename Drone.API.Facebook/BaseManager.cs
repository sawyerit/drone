using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.Facebook
{
	public class BaseManager
	{
		private XmlDocument xmlDoc = null;
		private string _graphUrl = string.Empty;

		public string GraphUrl
		{
			get
			{
				return _graphUrl;
			}
		}
		
		public BaseManager()
		{
			LoadXmlConfig();
		}

		private void LoadXmlConfig()
		{
			xmlDoc = new XmlDocument();
			string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Facebook.xml");

			try
			{
				xmlDoc.Load(sXMLPath);

				_graphUrl = XDocument.Parse(xmlDoc.InnerXml).Element("api").Element("graphurl").Value;
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.Facebook BaseManager"
																														, "BaseManager.LoadXmlConfig()"
																														, "nouser"
																														, System.Environment.MachineName
																														, "Path: " + sXMLPath));
			}
		}

	}
}
