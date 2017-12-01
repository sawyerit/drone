using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Drone.Shared;

namespace Drone.API.Dig
{
	internal static class Request
	{
		private static Collection<string> curQuery = new Collection<string>();

		internal static string ExecuteWebRequest(string query)
		{
			return ExecuteWebRequest(query, 100000);
		}

		/// <summary>
		/// Executes the webrequest and returns reponse text
		/// </summary>
		/// <param name="query">WebRequest uri query string</param>
		/// <param name="timeout">timeout in milliseconds</param>
		/// <returns></returns>
		internal static string ExecuteWebRequest(string query, int timeout)
		{
			WebResponse response = null;
			string responseText = string.Empty;

			try
			{
				WebRequest request = WebRequest.Create(query);
				request.Timeout = timeout;
				request.ContentType = "application/xml; charset=utf-8";
				response = (WebResponse)request.GetResponse();

				if (!Object.Equals(response, null))
					responseText = BuildTextFromResponse(response);
			}
			catch (WebException we)
			{
				//retry if it timed out, only once
				if (we.Message.Contains("timed out"))
				{
					if (curQuery.Contains(query))
					{
						return responseText;
					}
					else
					{
						curQuery.Add(query);
						ExecuteWebRequest(query, 10000);
					}
				}
				else
				{
					ExceptionExtensions.LogWarning(we.InnerException, "Drone.API.Dig.ExecuteWebRequest", query);
				}
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogWarning(e, "Drone.API.Dig.ExecuteWebRequest", query);
			}

			return responseText;
		}

		private static string BuildTextFromResponse(WebResponse response)
		{
			string responseText;
			using (var streamReader = new StreamReader(response.GetResponseStream()))
				responseText = streamReader.ReadToEnd();

			return responseText;
		}

	}
}
