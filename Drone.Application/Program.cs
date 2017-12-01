using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Drone.Manager;

namespace Drone.Application
{
	class Program
	{
		static void Main(string[] args)
		{
			DroneManager m = new DroneManager(args[0]);
			Thread thread = new Thread(m.Process);
			thread.Start(m);
		}
	}
}
