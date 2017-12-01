using System;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	/// <summary>
	/// Determines if the value exists in the address of the document
	/// </summary>
	/// <param name="dom">DocumentReader object for the domain</param>
	/// <param name="rule">The rule to match</param>
	/// <returns>True if the rule value is found in the website address.</returns>
	public class Urls : IProcessor
	{
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				return dom.Domain.Contains(rule.Value);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.Urls.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}

			return false;
		}
	}
}
