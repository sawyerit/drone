using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.API.Crunchbase;
using Drone.Entities.Crunchbase;
using Drone.Data.Queue;
using Drone.Shared;
using System.Xml;
using System.Messaging;
using Drone.API.Dig;
using Drone.Crunchbase.Datasources;
using Drone.Crunchbase.Components;
using QP = Drone.QueueProcessor.Components;
using Drone.QueueProcessor.Components;

namespace Drone.Test
{
	[TestClass]
	public class QueueProcessorTests
	{
		QueueManager _qm;
		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "queueprocessor");
			_qm = QueueManager.Instance; //new QueueManager(@".\Private$\Drone", new string[] { "Drone.Entities.Crunchbase.CompanyRoot, Drone.Entities.Crunchbase, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" });
		}
				
		[TestMethod]
		public void ReadCrunchFromQueue()
		{
			//QueueCrunchbase qc = new QueueCrunchbase(_qm);
			//_qm.GetFromQueue();
		}

		[TestMethod]
		public void QueueProcessor_ReadsAllFromQueue()
		{
			QP.QueueProcessor qp = new QP.QueueProcessor();
			qp.GetData(qp.Context);
		}
	}
}
