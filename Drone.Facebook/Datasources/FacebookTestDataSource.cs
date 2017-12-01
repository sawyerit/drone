using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Facebook.Components;
using System;
using Drone.Entities.Facebook;

namespace Drone.Facebook.Datasources
{
	public class FacebookTestDataSource : BaseDatasource<FacebookComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			FacebookDataComponent comp = component as FacebookDataComponent;
			if (!Object.Equals(comp, null))
				Shared.Utility.WriteToLogFile(String.Format("Facebook_TestDataRun_{0:M_d_yyyy}", DateTime.Today) + ".log", comp.FBPage.Name + ", " + comp.FBPage.Likes + ", " + DateTime.Now);
		}
	}
}
