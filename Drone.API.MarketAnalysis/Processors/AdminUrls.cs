using System;
using System.Net;
using System.Text.RegularExpressions;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class AdminUrls : IProcessor
	{
		/// <summary>
		/// Checks for the existence of an admin login page.
		/// </summary>
		/// <param name="dom">Domain to check, w/out the www.</param>
		/// <param name="rule">Rule that contains the admin page value to append to the domain. 
		/// Rule.Value is the admin page name that is appended to the domain.
		/// Rule.Property is the exact text to match on the returned admin page</param>
		/// <returns></returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www." + dom.Domain + "/" + rule.Value);

				request.UserAgent = "User-Agent	Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
				request.AllowAutoRedirect = true;
				request.Method = "GET";
				request.Timeout = 5000;
				HttpWebResponse resp = (HttpWebResponse)request.GetResponse();

				if (resp.StatusCode == HttpStatusCode.OK)
				{
					Regex r = new Regex(rule.Property, RegexOptions.IgnoreCase);
					HtmlAgilityPack.HtmlNode source = dom.Document.DocumentNode.SelectSingleNode("html");

					if (!Object.Equals(null, source))
						return r.Match(source.InnerHtml).Success;
				}

				return false;
			}
			catch (Exception)
			{
				Utility.WriteToLogFile(String.Format("SmallBiz_NoAdminInfo_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: {0}/{1}", dom.Domain, rule.Value));
			}

			return false;
		}
	}
}
