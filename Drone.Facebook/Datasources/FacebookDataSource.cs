using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Facebook;
using Drone.Entities.WebAPI;
using Drone.Shared;

namespace Drone.Facebook.Datasources
{
	public class FacebookDataSource : BaseDatasource<FacebookComponent>
	{
		JavaScriptSerializer _jserializer = new JavaScriptSerializer();

		public override void Process(IDroneDataComponent dataComponent)
		{
			FacebookDataComponent fbComponent = dataComponent as FacebookDataComponent;
			if (!Object.Equals(fbComponent, null))
			{
				if (!Object.Equals(fbComponent.FBPage, null))
					SendData(fbComponent.FBPage, "Page");

				if (!Object.Equals(fbComponent.CountryDemographic, null))
					SendData(fbComponent.CountryDemographic, "Country");

				if (!Object.Equals(fbComponent.LocaleDemographic, null))
					SendData(fbComponent.LocaleDemographic, "Locale");

				if (!Object.Equals(fbComponent.GenderDemographic, null))
					SendData(fbComponent.GenderDemographic, "Gender");
			}
		}

		public List<Competitor> GetCompetitorAccounts()
		{
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "commonapiuri") + "/competitors");
			_requestGet = (HttpWebRequest)WebRequest.Create(apiuri);
			_requestGet.Method = "GET";
			_requestGet.ContentType = "application/json";
			_requestGet.UseDefaultCredentials = true;

			string result = string.Empty;

			// Get response  
			using (HttpWebResponse response = _requestGet.GetResponse() as HttpWebResponse)
			{
				StreamReader reader = new StreamReader(response.GetResponseStream());
				result = reader.ReadToEnd();
			}

			List<Competitor> compList = new JavaScriptSerializer().Deserialize<List<Competitor>>(result);

			return compList;
		}

		private void SendData(object value, string typeName)
		{
			try
			{
				Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri") + "/" + typeName);
				HttpStatusCode code = SendRequest(apiuri, value, true);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "FacebookDataSource.SendRequest", typeName);
			}
		}

		#region private

		//private HttpWebRequest CreatePostRequest(string additionalUri)
		//{
		//	Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri") + additionalUri);

		//	HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiuri);
		//	req.Method = "POST";
		//	req.ContentType = "application/json";

		//	return req;
		//}

		//private string SendRequest(object value)
		//{
		//	string result = string.Empty;
		//	try
		//	{
		//		string requestData = _jserializer.Serialize(value);

		//		byte[] data = Encoding.UTF8.GetBytes(requestData);

		//		using (Stream dataStream = req.GetRequestStream())
		//		{
		//			dataStream.Write(data, 0, data.Length);
		//		}

		//		using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
		//		{
		//			result = new StreamReader(response.GetResponseStream()).ReadToEnd();
		//			if (response.StatusCode != HttpStatusCode.OK)
		//			{
		//				ExceptionExtensions.LogInformation("FacebookDataSource.SendRequest()", string.Format("Status Code: {0} RequetData: {1}", response.StatusCode, requestData));
		//			}
		//		}

		//	}
		//	catch (Exception e)
		//	{
		//		ExceptionExtensions.LogError(e, "FacebookDataSource.SendRequest", _requestPost.RequestUri.ToString());
		//	}
		//	return result;
		//}

		#endregion
	}
}
