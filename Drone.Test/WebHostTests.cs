using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.API.Dig;
using Drone.Shared;
using System.Linq;
using System.Threading.Tasks;
using Drone.MarketShare.Components;
using Drone.MarketShare.Datasources;
using Drone.Data.Queue;

namespace Drone.Test
{
	[TestClass]
	public class WebHostTests
	{
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marketshare");
		}

		[TestMethod]
		public void WebHost_GetData()
		{
			WebHost webHost = new WebHost(new MarketShareDataSource());
			webHost.GetData(webHost.Context);
		}

		[TestMethod]
		public void WebHost_GetData_MultiInstance()
		{
			//QueueManager qm = new QueueManager(@".\Private$\Drone", new string[] { "Drone.Entities.MarketShare.MarketShareDataType, Drone.Entities.MarketShare, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" });
			MarketShareDataSource smbd = new MarketShareDataSource();

			Parallel.For(0, 6, delegate(int i)
				{
					WebHost webHost = new WebHost();
					webHost.DroneDataSource = smbd;
					webHost.GetData(webHost.Context);
				}
			);
		
		}
	}
}
