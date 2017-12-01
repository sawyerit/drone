using Drone.Core;
using Drone.YouTube.Components;
using Drone.Core.Interfaces;
using System;
using Drone.Shared;
using System.Collections.Generic;
using System.Data;
using Drone.Entities.YouTube;

namespace Drone.YouTube.Datasources
{
	public class YouTubeTestDataSource : BaseDatasource<YouTubeComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			YouTubeDataComponent comp = component as YouTubeDataComponent;
			if (!Object.Equals(comp, null))
				//For now do nothing, this is a blank datasource for unit testing.
				Utility.WriteToLogFile(String.Format("YouTube_TestDataRun_{0:M_d_yyyy}", DateTime.Today) + ".log", comp.YouTubeChannel.Feed.Count + ", " + DateTime.Now);

		}
	}
}
