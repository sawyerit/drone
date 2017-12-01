using System;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class Scripts : IProcessor
	{
		/// <summary>
		/// Determines if the value exists in the Script tag collection of the document
		/// </summary>
		/// <param name="dom">DocumentReader object for the domain</param>
		/// <param name="rule">The rule to match</param>
		/// <returns>True if the rule value is found in the script tag collection.</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				foreach (var item in dom.JavascriptTags)
				{
					if (item.Attributes.Contains(rule.Property))
					{
						string propValue = item.Attributes[rule.Property].Value.ToLower();
						if (propValue.Contains(rule.Value.ToLower()))
							return true;
					}
				}
				return false;
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.Scripts.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}
			return false;
		}
	}
}
