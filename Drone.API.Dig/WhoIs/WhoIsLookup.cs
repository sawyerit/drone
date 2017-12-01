using System.Configuration;
using System.Xml;
using Drone.Shared;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Drone.API.Dig.WhoIs
{
	public class WhoIsLookup
	{
		private int i = 0;
		private List<string> _regServiceURLs = new List<string>();
		
		public List<string> RegServiceURLs
		{
			get { return _regServiceURLs; }
			set { _regServiceURLs = value; }
		}

		public WhoIsLookup()
		{}

		public string GetRegistrar(string domainName)
		{
			string results = string.Empty;
			string _regServiceURL = _regServiceURLs[++i % _regServiceURLs.Count];
			string registrar = "N/A";
			try
			{
				results = Request.ExecuteWebRequest(string.Format(_regServiceURL, domainName), 10000);

				if (results.Contains("failed") || results.Contains("unavailable"))
				{
					_regServiceURL = _regServiceURLs[++i % _regServiceURLs.Count];
					results = Request.ExecuteWebRequest(string.Format(_regServiceURL, domainName), 5000);
				}

				if (!string.IsNullOrEmpty(results))
					results = results.Replace("<pre>", string.Empty).Replace("</pre>", string.Empty).Replace("<br>", string.Empty);

				if (!string.IsNullOrEmpty(results) && !results.Contains("failed"))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(results);
                    //if (_regServiceURL.Contains("whois.godaddy.com"))
                    //{
                    //    results = XMLUtility.GetNodeFromXml(doc, "WHOIS/RESPONSE/DOMAIN/REGISTRAR").InnerText;
                    //}
                    //else
                    //{
						results = XMLUtility.GetNodeFromXml(doc, "WHOIS/WHOISINFOSTRUCT/REGISTRAR").InnerText;
					//}

					if (!string.IsNullOrEmpty(results))
						registrar = results;
				}
			}
			catch (Exception)
			{
				Utility.WriteToLogFile(String.Format("Dig_WhoIsFailure_{0:M_d_yyyy}", DateTime.Today) + ".log", "Domain: " + domainName);
			}

			return registrar;
		}
	}
}
