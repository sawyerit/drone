using System;

namespace Drone.Core
{
	public class BaseContext
	{
		public string Status { get; set; }
		public DateTime TimeOfStatus { get; set; }
		public TimeSpan DurationPreviousStatus { get; set; }		
		public DateTime NextRun { get; set; }
		public DateTime LastRun { get; set; }
		public bool ShuttingDown { get; set; }

		public void SetStatus(string status)
		{
			Status = status;
			DurationPreviousStatus = DateTime.Now.Subtract(TimeOfStatus);
			TimeOfStatus = DateTime.Now;
		}
	}
}
