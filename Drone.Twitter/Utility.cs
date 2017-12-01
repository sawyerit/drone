using System;
using System.Xml;
using System.Xml.XPath;
using Drone.API.Twitter.OAuth;
using Drone.Shared;

namespace Drone.Twitter
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
						AccessToken = nodeAccount.SelectSingleNode("token/accesstoken").FirstChild.Value,
						AccessTokenSecret = nodeAccount.SelectSingleNode("token/accesstokensecret").FirstChild.Value,
						ConsumerKey = nodeAccount.SelectSingleNode("token/consumerkey").FirstChild.Value,
						ConsumerSecret = nodeAccount.SelectSingleNode("token/consumersecret").FirstChild.Value,
					};
				}
				catch (Exception e)
				{
					ExceptionExtensions.LogError(e, "GetOAuthToken");
				}
			}

			return token;
		}
	}
}
