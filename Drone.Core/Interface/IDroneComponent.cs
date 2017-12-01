using System;

namespace Drone.Core.Interfaces
{
	public interface IDroneComponent
	{
		void GetData(object context);
		Type ComponentType { get; }
		void SetContextStatus(string status, BaseContext context);
		string GetContextStatus(BaseContext context);
		IDroneDataSource DroneDataSource { get; set; }
		IDroneDataComponent DroneDataComponent { get; set; }
		BaseContext Context { get; set; }

		event EventHandler ProcessingCompleted;
	}
}
