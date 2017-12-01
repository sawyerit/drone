using Drone.Core;
using Drone.Core.Interfaces;
using Drone.Entities.Twitter.v11;

namespace Drone.Twitter.Datasources
{	
	public class TwitterTestDataSource : BaseDatasource<TwitterComponent>
	{
		public override void Process(IDroneDataComponent component)
		{
			//For now do nothing, this is a blank datasource for unit testing.
		}
	}
}
