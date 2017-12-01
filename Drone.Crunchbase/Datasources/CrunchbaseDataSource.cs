using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Crunchbase;
using Drone.Shared;

namespace Drone.Crunchbase.Datasources
{
	public class CrunchbaseDataSource : BaseDatasource<CrunchbaseComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			CrunchbaseDataComponent crunchComponent = component as CrunchbaseDataComponent;
			if (!Object.Equals(crunchComponent, null))
			{
				SendCompany(crunchComponent.CompanyLocal);
			}
		}

		private void SendCompany(CompanyRoot companyRoot)
		{
			//CreatePostRequest
			Uri apiuri = new Uri(XMLUtility.GetTextFromAccountNode(Xml, "apiuri"));

			//send
			try
			{
				HttpStatusCode code = SendRequest(apiuri, companyRoot, true);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "CrunchbaseDataSource.SendRequest", companyRoot.permalink);
			}
		}
	}
}
