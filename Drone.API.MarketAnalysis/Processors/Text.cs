using System;
using System.Text.RegularExpressions;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class Text : IProcessor
	{
		/// <summary>
		/// Uses RegEx to determines if the value exists in the text of the document source.
		/// </summary>
		/// <param name="dom">DocumentReader object for the domain</param>
		/// <param name="rule">The rule to match</param>
		/// <returns>True if the rule value is found in the source of the document.</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{									
				Regex r = new Regex(rule.Value.Replace("{domain}", "www." + dom.Domain), RegexOptions.IgnoreCase);
				HtmlAgilityPack.HtmlNode source = dom.Document.DocumentNode.SelectSingleNode(rule.Property);

				if (!Object.Equals(null, source))
					return r.Matches(source.InnerHtml).Count > 0;
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.Text.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}

			return false;
		}
	}
}
