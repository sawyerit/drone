using System;
using System.Xml;
using System.Xml.XPath;
using Drone.API.Facebook.OAuth;
using Drone.Shared;

namespace Drone.Facebook
{
	public static class Utility
	{
		internal static OAuthTokens GetOAuthToken(IXPathNavigable Xml)
		{
			XmlDocument xmlDoc = Xml as XmlDocument;
			OAuthTokens token = new OAuthTokens();

			if (!Object.Equals(xmlDoc, null))
			{
				try
				{
					XmlNode nodeAccount = xmlDoc.SelectSingleNode(String.Format("/accounts/account/name[text()='{0}']/..", "GoDaddy"));

					token = new OAuthTokens
					{					
						AccessToken = nodeAccount.SelectSingleNode("accesstoken").FirstChild.Value,
						AccessID = nodeAccount.SelectSingleNode("id").FirstChild.Value
					};
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "Facebook.Utility.GetOAuthToken");
				}
			}

			return token;
		}
	}
}
