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
using Drone.Data;
using System.Data;

namespace Drone.Test
{
    [TestClass]
    public class TeraDataTests
    {
        [TestInitialize]
        public void Setup() { }

        [TestMethod]
        public void TeraDataFactory_DataSetTest()
        {
            DataSet ds = TDDataFactory.ExecuteDataSet("SELECT DATE");
            Assert.IsNotNull(ds);
            Assert.AreEqual(ds.Tables.Count, 1);
            Assert.AreEqual(ds.Tables[0].Rows.Count, 1);
        }

        [TestMethod]
        public void TeraDataFactory_ExecuteScalarTest()
        {
            int i = TDDataFactory.ExecuteScalar<int>("SELECT '1'");
            Assert.AreEqual<int>(1, i);

            DateTime dt = TDDataFactory.ExecuteScalar<DateTime>("SELECT DATE");
            Assert.IsNotNull(dt);

            string  s = TDDataFactory.ExecuteScalar<string>("SELECT DATE");
            Assert.IsTrue(!String.IsNullOrEmpty(s));
        }
    }
}
