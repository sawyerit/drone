using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Drone.API.Twitter.OAuth;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.Twitter
{
	internal static class Request
	{
		internal static string ExecuteAnonymousWebRequest(string query)
		{
			HttpWebResponse response = null;
			string responseText = string.Empty;

			try
			{
				WebRequest request = WebRequest.Create(query);
				request.ContentType = "application/json; charset=utf-8";
				response = (HttpWebResponse)request.GetResponse();

				if (!Object.Equals(response, null))
					responseText = BuildTextFromResponse(response);
			}
			catch (WebException we)
			{
				response = we.Response as HttpWebResponse;
				if (!Object.Equals(response, null) && (int)response.StatusCode == 420)
					Thread.Sleep(300000); //if we are rate limited, wait 5 min before proceeding
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.API.Twitter.Request.ExecuteAnonymousRequest", query);
			}

			return responseText;
		}

		internal static string ExecuteAuthenticatedWebRequest(string twitterQuery, OAuthTokens tokens, bool retry = true)
		{
			//get mentions
			string responseText = string.Empty;
			WebResponse response;
			try
			{
				WebRequestBuilder requestBuilder = new WebRequestBuilder(new Uri(twitterQuery), HTTPVerb.GET, tokens);
				response = requestBuilder.ExecuteRequest();

				if (!Object.Equals(response, null))
				{
					responseText = BuildTextFromResponse(response);

					if (string.IsNullOrEmpty(responseText))
						ExceptionExtensions.LogWarning(new ArgumentNullException("Twitter responseText from the httpwebresponse was empty"), "Twitter ExecuteAuthenticatedWebRequest", "Query: " + twitterQuery);
				}
				else
				{
					ExceptionExtensions.LogWarning(new ArgumentNullException("Twitter httwebresponse object was null"), "Twitter ExecuteAuthenticatedWebRequest", "Query: " + twitterQuery);
				}
			}
			catch (Exception e)
			{
				if (retry)
				{
					if (e.Message.Contains("429") || e.Message.Contains("420"))
					{
                        ExceptionExtensions.LogWarning(e, "Twitter.Request.ExecuteAuthenticatedWebRequest");
						Thread.Sleep(300000); //wait 5 min because of rate limiting                        
					}
					else
					{
						Thread.Sleep(60000);
					}
					ExecuteAuthenticatedWebRequest(twitterQuery, tokens, false);
				}
				else
				{
					ExceptionExtensions.LogError(e, "Drone.API.Twitter.Request.ExecuteAuthenticatedWebRequest", twitterQuery);
				}
			}

			return responseText;
		}

		internal static string ExecuteAuthenticatedWebRequestPost(string twitterQuery, OAuthTokens tokens, bool retry = true)
		{
			//get mentions
			string responseText = string.Empty;
			WebResponse response;
			try
			{
				WebRequestBuilder requestBuilder = new WebRequestBuilder(new Uri(twitterQuery), HTTPVerb.POST, tokens);
				requestBuilder.Multipart = true;

				response = requestBuilder.ExecuteRequest();

				if (!Object.Equals(response, null))
				{
					responseText = BuildTextFromResponse(response);

					if (string.IsNullOrEmpty(responseText))
						ExceptionExtensions.LogWarning(new ArgumentNullException("Twitter responseText from the httpwebresponse was empty"), "Twitter ExecuteAuthenticatedWebRequestPost", "Query: " + twitterQuery);
				}
				else
				{
					ExceptionExtensions.LogWarning(new ArgumentNullException("Twitter httwebresponse object was null"), "Twitter ExecuteAuthenticatedWebRequestPost", "Query: " + twitterQuery);
				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("404") || e.Message.Contains("502"))
				{
					Thread.Sleep(1000);
					ExecuteAuthenticatedWebRequestPost(twitterQuery, tokens, true);
				}
				else
				{
					ExceptionExtensions.LogError(e, "Drone.API.Twitter.Request.ExecuteAuthenticatedWebRequestPOST", twitterQuery); 
				}
			}

			return responseText;
		}

		/// <summary>
		/// Javascript deserializer to convert response text to object
		/// </summary>
		/// <typeparam name="T">object to deserialize to</typeparam>
		/// <param name="responseText">text from web request call</param>
		/// <returns></returns>
		internal static T Deserialize<T>(string responseText) where T : new()
		{
			T temp = new T();

			try
			{
				if (!string.IsNullOrEmpty(responseText))
					temp = new JavaScriptSerializer().Deserialize<T>(responseText);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.API.Twitter.Request.Deserialize", responseText);
			}

			return temp;
		}

		private static string BuildTextFromResponse(WebResponse response)
		{
			string responseText = string.Empty;

			using (var streamReader = new StreamReader(response.GetResponseStream()))
				responseText = streamReader.ReadToEnd();

			return responseText;
		}
	}
}
