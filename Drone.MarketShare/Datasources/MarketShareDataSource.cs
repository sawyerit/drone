using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.MarketShare;
using Drone.Entities.WebAPI;
using Drone.Shared;

namespace Drone.MarketShare.Datasources
{
	public class MarketShareDataSource : BaseDatasource<MarketShareComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			MarketShareDataComponent smbComponent = component as MarketShareDataComponent;
			if (!Object.Equals(smbComponent, null))
			{
				SendDomainData(smbComponent.MarketShareType);
			}
		}


		#region helpers 

		private void SendDomainData(MarketShareDataType msType)
		{
			//CreatePostRequest			
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri"));

			//send
			try
			{					
				HttpStatusCode code = SendRequest(apiuri, msType, true);
				if (code != HttpStatusCode.Created)
				{
					ExceptionExtensions.LogInformation("MarketShareDataSource.SendDomainData", "Send failed with status code: " + code + ". Trying again.");
					SendDomainData(msType);
				}				
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "MarketShareDataSource.SendRequest()", "DomainID: " + msType.DomainId);
			}
		}

		#endregion

		#region lookup data

		public List<Domain> GetAllCompanies(MarketShareTypeBitMaskEnum marketTypeBitMaskEnum, int rows)
		{
			string result = string.Empty;
			HttpWebRequest requestGet = (HttpWebRequest)WebRequest.Create(string.Format("{0}?count={1}&mask={2}"
																																		, XMLUtility.GetTextFromAccountNode(Xml, "domainsapiuri")
																																		, rows
																																		, (int)marketTypeBitMaskEnum));
			requestGet.UseDefaultCredentials = true;

			// Get response  
			using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
			{
				StreamReader reader = new StreamReader(response.GetResponseStream());
				result = reader.ReadToEnd();
			}

			List<Domain> domainList = new JavaScriptSerializer().Deserialize<List<Domain>>(result);

			return domainList;
		}

		#endregion
	}
}
