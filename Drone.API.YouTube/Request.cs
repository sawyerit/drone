using System;
using System.IO;
using System.Net;
using System.Threading;
using Drone.Shared;
using Drone.Shared.LoggingService;
using System.Web.Script.Serialization;

namespace Drone.API.YouTube
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
				request.ContentType = "text/xml; charset=utf-8";
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
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.YouTube Exception"
																														, "ExecuteAnonymousWebRequest "
																														, "nouser"
																														, System.Environment.MachineName));
			}

			return responseText;
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
