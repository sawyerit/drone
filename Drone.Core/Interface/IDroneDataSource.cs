using System;
using System.Collections.Generic;
using System.Net;

namespace Drone.Core.Interfaces
{
	public interface IDroneDataSource
	{
		void Process(IDroneDataComponent component);
		Type ComponentType { get; }
	}
}
