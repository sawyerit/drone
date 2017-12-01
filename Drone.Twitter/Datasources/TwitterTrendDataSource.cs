using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;

namespace Drone.Twitter.Datasources
{
	public class TwitterTrendDataSource : BaseDatasource<TwitterComponent>
	{
		//write to WebAPI service if this is to be used again
		public override void Process(IDroneDataComponent component)
		{
			TwitterDataComponent twitterDataComponent = component as TwitterDataComponent;

			//if (!Object.Equals(twitterDataComponent, null))
			//	SaveTrendData(twitterDataComponent);
		}
	}
}
