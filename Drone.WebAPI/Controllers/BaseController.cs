using System.Collections.Generic;
using System.Web.Http;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Interfaces;
using System;

namespace Drone.WebAPI.Controllers
{
    public class BaseController : ApiController
    {
        public static int CurrentRequestCount = 0;
        public static int CrawlDaddyRequestCount = 0;
        public static string RequestCountLastReset = DateTime.Now.ToString();
        private static System.Timers.Timer _countTimer = null;

        public BaseController()
        {
            if (_countTimer == null)
            {
                _countTimer = new System.Timers.Timer();
                _countTimer.Elapsed += _countTimer_Elapsed;
                _countTimer.Interval = 300000;
                _countTimer.Start();
            }
        }

        void _countTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _countTimer.Stop();
            CurrentRequestCount = 0;
            CrawlDaddyRequestCount = 0;
            RequestCountLastReset = DateTime.Now.ToString();
            _countTimer.Start();
        }

        internal void IncrementCount()
        {
            CurrentRequestCount++;
        }

        internal void IncrementCrawlDaddyCount()
        {
            CrawlDaddyRequestCount++;
        }
    }
}