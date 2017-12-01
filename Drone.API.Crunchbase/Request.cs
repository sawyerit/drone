using System;
using System.IO;
using System.Net;
using System.Threading;
using Drone.Shared;
using Drone.Shared.LoggingService;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Drone.API.Crunchbase
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
					Thread.Sleep(300000); //if we are rate limited, wait ??????
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "Drone.API.Crunchbase.ExecuteAnonymousWebRequest");
			}

			return responseText;
		}

		/// <summary>
		/// Javascript deserializer to convert response text to object
		/// </summary>
		/// <typeparam name="T">object to deserialize to</typeparam>
		/// <param name="responseText">text from web request call</param>
		/// <returns></returns>
		public static T Deserialize<T>(string json, bool verboseLog)
		{
			T returnValue = default(T);
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream();

				byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
				memoryStream.Write(jsonBytes, 0, jsonBytes.Length);
				memoryStream.Seek(0, SeekOrigin.Begin);
				using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(memoryStream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null))
				{
					var serializer = new DataContractJsonSerializer(typeof(T));
					returnValue = (T)serializer.ReadObject(jsonReader);

				}
			}
			catch (Exception e)
			{
				Utility.WriteToLogFile(String.Format("Crunchbase_DeserializeFailure_{0:M_d_yyyy}", DateTime.Today) + ".log", (verboseLog) ? "JSON: " + json : "Deserialize Error");
			}
			finally
			{
				if (!Object.Equals(null, memoryStream))
					memoryStream.Dispose();
			}
			return returnValue;
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
