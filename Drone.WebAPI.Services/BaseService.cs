using Drone.Data.Queue;

namespace Drone.WebAPI.Services
{
	public class BaseService
	{
		public QueueManager _queueManager;

		public BaseService()
		{
			_queueManager = QueueManager.Instance;
		}
	}
}
