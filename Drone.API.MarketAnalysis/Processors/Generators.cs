using System;
using Drone.Shared;

namespace Drone.API.MarketAnalysis
{
	public class Generators : IProcessor
	{
		/// <summary>
		/// Determines if the Generator tag contains the value in the rule
		/// </summary>
		/// <param name="dom">DocumentReader object for the domain</param>
		/// <param name="rule">The rule to match</param>
		/// <returns>True if the rule value is found in the generator tag collection</returns>
		public bool Process(DOMReader dom, MarketShareRule rule)
		{
			try
			{
				foreach (HtmlAgilityPack.HtmlNode node in dom.GeneratorTags)
				{
					if (!Object.Equals(null, node.Attributes[rule.Property]))
						if (node.Attributes[rule.Property].Value.ToLower().Contains(rule.Value.ToLower()))
							return true;
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "API.MarketAnalysis.Generators.Process()", string.Format("Domain: {0}, {1}", dom.Domain, rule.ToString()));
			}

			return false;
		}
	}
}
