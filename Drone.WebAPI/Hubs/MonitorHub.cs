using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using Drone.WebAPI.Services;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Controllers;
using System.Configuration;

namespace Drone.WebAPI.Hubs
{
    public class MonitorHub : Hub
    {
        private static Timer _timer = null;
        private static TimeSpan _updateInterval;

        public MonitorHub()
        {
            if (Object.Equals(null, _timer))
            {
                _updateInterval = TimeSpan.FromMilliseconds(Convert.ToInt32(ConfigurationManager.AppSettings["MonitorInterval"]));
                _timer = new Timer(BroadcastStatus, null, _updateInterval, _updateInterval);                
            }
        }

        public void Send(string command, string service)
        {
            MonitorService.RunCommand(command, service);
        }

        private void BroadcastStatus(object state)
        {
            Status s = MonitorService.GetStatus();
            s.RequestCount = string.Format("{0} - {1}", BaseController.CurrentRequestCount, BaseController.RequestCountLastReset);
            s.CrawlDaddyRequestCount = string.Format("{0} - {1}", BaseController.CrawlDaddyRequestCount, BaseController.RequestCountLastReset);
            s.RequestRate = GetRequestRate(BaseController.CurrentRequestCount, BaseController.RequestCountLastReset).ToString();
            s.CrawlDaddyRequestRate = GetRequestRatePerMin(BaseController.CrawlDaddyRequestCount, BaseController.RequestCountLastReset).ToString();
            Clients.All.showStatus(s);
        }

        private double GetRequestRatePerMin(int curCount, string lastReset)
        {
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(lastReset);
            if (ts.TotalSeconds == 0)
                return 0;

            return Math.Round((curCount / ts.TotalSeconds) * 60);
        }

        private double GetRequestRate(int curCount, string lastReset)
        {
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(lastReset);
            if (ts.TotalSeconds == 0)
                return 0;

            return Math.Round(curCount / ts.TotalSeconds);
        }
    }
}