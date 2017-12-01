using System;
using System.Xml;
using System.Xml.XPath;

namespace Drone.Shared
{
	public static class XMLUtility
	{
		public static XmlNode GetNodeFromXml(IXPathNavigable Xml, string p)
		{
			XmlDocument xmlDoc = Xml as XmlDocument;
			return GetNodeFromXml(xmlDoc, p);
		}

		public static XmlNode GetNodeFromXml(XmlDocument xmlDoc, string p)
		{
			XmlNode xNode = null;

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					xNode = xmlDoc.SelectSingleNode(p);
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "Utility.GetNodeFromXml", "Node: " + p);
				}

				if (Object.Equals(xNode, null))
					ExceptionExtensions.LogError(new ArgumentNullException("Node not found in XML - XPath: " + p), "Utility.GetNodeFromXml");
			}
			else
			{
				ExceptionExtensions.LogError(new ArgumentNullException("XML document was null. XPath: " + p), "Utility.GetNodeFromXml");
			}

			return xNode;
		}

		public static XmlNode GetNodeFromNode(XmlNode nodeAccount, string p)
		{
			XmlNode xNode = null;

			try
			{
				xNode = nodeAccount.SelectSingleNode(p);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Utility.GetNodeFromNode");
			}

			if (Object.Equals(xNode, null))
				ExceptionExtensions.LogError(new ArgumentNullException("Node not found in XML - XPath: " + p), "Utility.GetNodeFromNode");

			return xNode;
		}

		public static string GetTextFromAccountNode(IXPathNavigable Xml, string expressionPath)
		{
			string nodeText = string.Empty;
			XmlDocument xmlDoc = Xml as XmlDocument;

			XmlNode xNode = xmlDoc.SelectSingleNode("/accounts/account/name[text()='GoDaddy']/../" + expressionPath).FirstChild;
			if (!Object.Equals(xNode, null))
				nodeText = xNode.Value;

			return nodeText;
		}

		public static int GetIntFromAccountNode(IXPathNavigable Xml, string expressionPath)
		{
			return GetTextFromAccountNode(Xml, expressionPath).ConvertStringToInt(0);
		}

		public static Boolean IsEnabled(IXPathNavigable Xml)
		{
			bool returnVal = false;

			try
			{
				XmlNode childNode = null;
				XmlNode nodeAccount = GetNodeFromXml(Xml, String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));

				if (!Object.Equals(nodeAccount, null))
				{
					childNode = GetNodeFromNode(nodeAccount, "isenabled");
					if (!Object.Equals(childNode, null))
						bool.TryParse(childNode.FirstChild.Value, out returnVal);
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "IsEnabled");
			}

			return returnVal;
		}

		public static bool IsComponentEnabled(IXPathNavigable Xml, string nodeMatch)
		{
			bool returnVal = false;
			XmlDocument xmlDoc = Xml as XmlDocument;

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode childNode = null;
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));

					if (!Object.Equals(nodeAccount, null))
					{
						childNode = GetNodeFromNode(nodeAccount, nodeMatch);
						if (!Object.Equals(childNode, null))
							bool.TryParse(GetNodeFromNode(childNode, "isenabled").FirstChild.Value, out returnVal);
					}
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "IsComponentEnabled");
				}

			}

			return returnVal;
		}

		public static DateTime GetNextRunInterval(IXPathNavigable Xml)
		{
			DateTime nextInterval = DateTime.Now.AddSeconds(120);

			XmlDocument xmlDoc = Xml as XmlDocument;
			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));
					nextInterval = DateTime.Now.AddSeconds(nodeAccount.SelectSingleNode("nextruninterval").FirstChild.Value.ConvertStringToInt(120));
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "GetNextRunInterval", "Twitter next run interval missing");
				}
			}

			return nextInterval;
		}

		public static DateTime GetNext15MinRunTime()
		{
			DateTime nextInterval = DateTime.Now.AddSeconds(120);
			DateTime nowish = DateTime.Now;

			nextInterval = new DateTime(nowish.Year, nowish.Month, nowish.Day, nowish.Hour, nowish.Minute, 0).AddMinutes(15 - nowish.Minute % 15);

			return nextInterval;
		}

		public static DateTime GetNextRunIntervalByNode(IXPathNavigable Xml, string nodeMatch)
		{
			DateTime nextInterval = DateTime.Now.AddSeconds(120);

			XmlDocument xmlDoc = Xml as XmlDocument;

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));
					string nodeValue = nodeAccount.SelectSingleNode(nodeMatch + "/interval").FirstChild.Value;

					switch (nodeValue)
					{
						case "1440":
						case "daily":
							nextInterval = DateTime.Today.AddDays(1);
							break;
						case "weekly":
							nextInterval = DateTime.Today.AddDays(7);
							break;
						case "sunday":
						case "monday":
						case "tuesday":
						case "wednesday":
						case "thursday":
						case "friday":
						case "saturday":
							nextInterval = DateTime.Now.Next((DayOfWeek)Enum.Parse(typeof(DayOfWeek), nodeValue, true)).Date;
							break;
						default:
							nextInterval = DateTime.Now.AddMinutes(nodeValue.ConvertStringToInt(20));
							break;
					}
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "GetNextRunIntervalByNode", "Missing node: " + nodeMatch);
				}

			}

			return nextInterval;
		}

		public static long GetUserId(IXPathNavigable Xml)
		{
			XmlDocument xmlDoc = Xml as XmlDocument;
			long returnVal = 0;

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeName = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']", "GoDaddy"));
					returnVal = long.Parse(nodeName.NextSibling.FirstChild.Value);
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "GetUserId");
				}
			}

			return returnVal;
		}

		public static void GetPageResultCounts(IXPathNavigable Xml, string configNode, out int countPerPage, out int pageCount, int defaultCountPerPage, int defaultPageCount)
		{
			pageCount = defaultPageCount;
			countPerPage = defaultCountPerPage;

			XmlDocument xmlDoc = Xml as XmlDocument;
			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));
					if (!Object.Equals(nodeAccount, null))
					{
						countPerPage = nodeAccount.SelectSingleNode(configNode + "/countperpage").FirstChild.Value.ConvertStringToInt(defaultCountPerPage);
						pageCount = nodeAccount.SelectSingleNode(configNode + "/pagecount").FirstChild.Value.ConvertStringToInt(defaultPageCount);
					}
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "GetPageResultCounts");
				}
			}
		}

		public static bool UseSinceId(IXPathNavigable Xml, string configNode)
		{
			XmlDocument xmlDoc = Xml as XmlDocument;
			bool resultBool = false;

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));
					bool.TryParse(nodeAccount.SelectSingleNode(configNode + "/usesinceid").FirstChild.Value, out resultBool);
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "UseSinceId");
				}
			}

			return resultBool;
		}

	}
}
