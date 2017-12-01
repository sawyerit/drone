﻿using System;
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
	public class EmailHostTests
	{
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marketshare");
		}

		[TestMethod]
		public void EmailHost_GetData()
		{
			EmailHost emailHost = new EmailHost(new MarketShareDataSource());
			emailHost.GetData(emailHost.Context);
		}

		[TestMethod]
		public void EmailHost_GetData_WithQueue()
		{
			//QueueManager qm = new QueueManager(@".\Private$\Drone", new string[] { "Drone.Entities.MarketShare.MarketShareDataType, Drone.Entities.MarketShare, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" });
			EmailHost emailHost = new EmailHost();
			emailHost.DroneDataSource = new MarketShareTestDataSource();

			emailHost.GetData(emailHost.Context);
		}
	}
}
