using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drone.Entities.WebAPI
{
    public class Status
    {
        private List<DroneService> _droneServices = new List<DroneService>();
        public List<DroneService> DroneServices
        {
            get { return _droneServices; }
            set { _droneServices = value; }
        }

        private QueueInfo _queue;
        public QueueInfo Queue
        {
            get { return _queue; }
            set { _queue = value; }
        }

        public string ServiceBoxName { get; set; }

        public string RequestCount { get; set; }
        public string CrawlDaddyRequestCount { get; set; }
        public string RequestRate { get; set; }
        public string CrawlDaddyRequestRate { get; set; }
    }

    public class DroneService
    {
        public string ServiceName { get; set; }            
        public string Status { get; set; }
        public bool HasError { get; set; }
        public string ServiceShortName
        {
            get { return ServiceName.Replace("Drone.", "").Replace(".Service", ""); }
        }
        public string StatusAction
        {
            get { return Status == "Running" ? "Stop" : "Start"; }
        }
    }
}
