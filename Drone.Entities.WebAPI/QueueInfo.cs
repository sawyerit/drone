using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Drone.Entities.WebAPI
{
	public class QueueInfo
	{
		public string QueueName { get; set; }
		public string ServerName { get; set; }
		public int QueueCount { get; set; }
		public bool HasErrors { get; set; }
	}
}