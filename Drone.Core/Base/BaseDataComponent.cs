using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Core.Interfaces;

namespace Drone.Core
{
	public class BaseDataComponent<TComponentType> : IDroneDataComponent
	{
		public Type ComponentType { get; set; }

		public static string ComponentTypeString { get; set; }

		
		public BaseDataComponent()
		{
			ComponentType = typeof(TComponentType);
		}
	}
}