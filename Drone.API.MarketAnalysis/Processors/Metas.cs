using System;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class Metas : IProcessor
	{
		/// <summary>
		/// Determines if the value exists in the Meta tag collection of the document
		/// </summary>
		/// <param name="dom">DocumentReader object for the domain</param>
		/// <param name="rule">The rule to match</param>
		/// <returns>True if the rule value is found in the meta tag collection.</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				return dom.ExistsInCollection(dom.MetaTags, rule);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.Metas.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}
			return false;
		}
	}
}
