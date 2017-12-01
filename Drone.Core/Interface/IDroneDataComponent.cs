using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Core.Interfaces
{
	public interface IDroneDataComponent
	{
		Type ComponentType { get; }
	}
}
