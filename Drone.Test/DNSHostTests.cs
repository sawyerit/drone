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
	public class DNSHostTests
	{
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marketshare");
		}

		[TestMethod]
		public void DNSHost_GetData()
		{
			DNSHost dnsHost = new DNSHost(new MarketShareDataSource());
			dnsHost.GetData(dnsHost.Context);
		}

		[TestMethod]
		public void DNSHost_GetData_WithQueue()
		{
			//QueueManager qm = new QueueManager(@".\Private$\Drone", new string[] { "Drone.Entities.MarketShare.MarketShareDataType, Drone.Entities.MarketShare, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" });
			DNSHost dnsHost = new DNSHost();
			dnsHost.DroneDataSource = new MarketShareTestDataSource();

			dnsHost.GetData(dnsHost.Context);
		}
	}
}
