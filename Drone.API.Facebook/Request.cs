using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Drone.Shared;
using Drone.Shared.LoggingService;

namespace Drone.API.Facebook
{
	internal static class Request
	{
		internal static string ExecuteWebRequest(string query)
		{
			WebResponse response = null;
			string responseText = string.Empty;

			try
			{
				WebRequest request = WebRequest.Create(query);
				request.ContentType = "application/json; charset=utf-8";
				response = (WebResponse)request.GetResponse();

				if (!Object.Equals(response, null))
					responseText = BuildTextFromResponse(response);
			}
			catch (Exception e)
			{
				using (var logClient = new BILoggerServiceClient())
					logClient.HandleBIException(e.ConvertToBIException(LogActionEnum.Log
																														, LogTypeEnum.Error
																														, "Drone.API.Facebook Exception"
																														, "ExecuteWebRequest "
																														, "nouser"
																														, System.Environment.MachineName
																														, "Query " + query));
			}

			return responseText;
		}

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
				ExceptionExtensions.LogError(e, "Facebook.Deserialize", responseText);
			}

			return temp;
		}

		internal static T Deserialize<T>(string responseText, JavaScriptConverter customConverter) where T : new()
		{
			T temp = new T();

			if (!string.IsNullOrEmpty(responseText))
			{
				var serializer = new JavaScriptSerializer();
				serializer.RegisterConverters(new[] { customConverter });

				temp = serializer.Deserialize<T>(responseText);
			}
			return temp;
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
