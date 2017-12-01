using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;
using Drone.Entities.WebAPI;
using Drone.Shared;

namespace Drone.Twitter.Datasources
{
	public class TwitterUserDataSource : BaseDatasource<TwitterComponent>
	{
		JavaScriptSerializer _jserializer = new JavaScriptSerializer();

		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;

			if (!Object.Equals(twitterDataComponent.TwitterUserList, null))
				SendFollowerData(twitterDataComponent.TwitterUserList);
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

		private void SendFollowerData(List<User> list)
		{
			//CreatePostRequest
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri") + "/users");

			//send
			try
			{
				HttpStatusCode code = SendRequest(apiuri, list, true);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "TwitterDataSource.SendFollowerData");
			}
		}
	}
}
