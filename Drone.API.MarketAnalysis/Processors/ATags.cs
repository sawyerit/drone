using System;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class ATags : IProcessor
	{
		/// <summary>
		/// Determines if the rule value exists in the collection of A tags in the document
		/// </summary>
		/// <param name="dom">DocumentReader object that contains the html</param>
		/// <param name="rule">Rule defined in XML</param>
		/// <returns>True if Rule.Value exists in ATag collection</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				return dom.ExistsInCollection(dom.ATags, rule);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.ATags.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}
			return false;
		}
	}
}
