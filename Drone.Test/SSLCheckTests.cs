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
	public class SSLCheckTests
	{
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marketshare");
		}

		[TestMethod]
		public void SSLCheck_GetData()
		{
			SSLCheck sslCheck = new SSLCheck(new MarketShareDataSource());
			sslCheck.GetData(sslCheck.Context);
		}
	}
}
