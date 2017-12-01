using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.WebAPI;
using Drone.Entities.YouTube;
using Drone.Shared;

namespace Drone.YouTube.Datasources
{
	public class YouTubeDataSource : BaseDatasource<YouTubeComponent>
	{
		JavaScriptSerializer _jserializer = new JavaScriptSerializer();

		public override void Process(IDroneDataComponent component)
		{
			YouTubeDataComponent ytdc = component as YouTubeDataComponent;
			if (!Object.Equals(ytdc, null))
			{
				SendChannelInfo(ytdc);
			}
		}

		public List<Competitor> GetCompetitorAccounts()
		{
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "commonapiuri") + "/competitors");
			_requestGet = (HttpWebRequest)WebRequest.Create(apiuri);
			_requestGet.Method = "GET";
			_requestGet.ContentType = "application/json";
			_requestGet.UseDefaultCredentials = true;
            _requestGet.PreAuthenticate = true;

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

		#region private


		private void SendChannelInfo(YouTubeDataComponent ytdc)
		{
			//CreatePostRequest
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri"));

			//send
			try
			{
				HttpStatusCode code = SendRequest(apiuri, ytdc.YouTubeChannel, true);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "YouTubeDataSource.SendRequest", ytdc.YouTubeChannel.Name);
			}
		}

		#endregion
	}
}
