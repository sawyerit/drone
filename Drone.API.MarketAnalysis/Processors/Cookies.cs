using System;
using System.Net;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class Cookies : IProcessor
	{
		/// <summary>
		/// Gets the cookies for the domain and determines if there is a rule match
		/// </summary>
		/// <param name="dom">The DocumentReader object for the domain</param>
		/// <param name="rule">The rule to match</param>
		/// <returns>True if a cookie is found matching the rule</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{

				if (Object.Equals(null, dom.RequestCookies) || Object.Equals(null, dom.ResponseCookies))
				{
					//get cookies and look for a match
					HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www." + dom.Domain);

					CookieContainer cookieJar = new CookieContainer();
					request.UserAgent = "User-Agent	Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
					request.CookieContainer = cookieJar;
					request.AllowAutoRedirect = true;
					request.Method = "GET";
					request.Timeout = 5000;
					HttpWebResponse resp = (HttpWebResponse)request.GetResponse();

					dom.RequestCookies = cookieJar.GetCookies(request.RequestUri);
					dom.ResponseCookies = cookieJar.GetCookies(resp.ResponseUri);
				}

				foreach (Cookie c1 in dom.RequestCookies)
					if (c1.Name.Contains(rule.Value))
						return true;

				foreach (Cookie c in dom.ResponseCookies)
					if (c.Name.Contains(rule.Value))
						return true;
			}
			catch (Exception e)
			{
				if (!e.Message.Contains("404") && !e.Message.Contains("timed out"))
				{
					Utility.WriteToLogFile(String.Format("SmallBiz_NoCookieInfo_{0:M_d_yyyy}", DateTime.Today) + ".log", string.Format("Domain: {0}", dom.Domain));
				}
			}

			return false;
		}
	}
}
