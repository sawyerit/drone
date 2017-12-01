using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Company.SqlLib;
using Drone.Shared;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using MongoDB.Driver.Builders;
//using Teradata.Client.Provider;

namespace Drone.Data
{
    public static class TDDataFactory
    {
        //private static TdConnection tdConn;
        private static string tdConnString = ConfigurationManager.AppSettings.Get("TeraData");

        public static DataSet ExecuteDataSet(string query)
        {
            DataSet ds = new DataSet();
            //using (tdConn = new TdConnection(tdConnString))
            //{
            //    tdConn.Open();
            //    TdCommand cmd = tdConn.CreateCommand();
            //    cmd.CommandText = query;
            //    TdDataAdapter da = new TdDataAdapter(cmd);

            //    da.Fill(ds);
            //}
            return ds;
        }

        public static int ExecuteNonQuery(string query)
        {
            int rowsAffected = 0;
            //using (tdConn = new TdConnection(tdConnString))
            //{
            //    tdConn.Open();
            //    TdCommand cmd = tdConn.CreateCommand();
            //    cmd.CommandText = query;
            //    rowsAffected = cmd.ExecuteNonQuery();
            //}
            return rowsAffected;
        }

        public static T ExecuteScalar<T>(string query)
        {
            T retVal = default(T);

            //using (tdConn = new TdConnection(tdConnString))
            //{
            //    tdConn.Open();
            //    TdCommand cmd = tdConn.CreateCommand();
            //    cmd.CommandText = query;
            //    retVal = Conversions.ConvertTo(cmd.ExecuteScalar(), default(T));
            //}

            return retVal;
        }      
    }
}
//Reader
////"Data Source=10.1.228.60;User ID=na6kMmEso8Ks44V;Password=RYmst4fpssk5HyG"
//using (tdConn)
//{
//    tdConn.Open();
//    TdCommand cmd = tdConn.CreateCommand();
//    cmd.CommandText = "SELECT DATE";

//    using (TdDataReader reader = cmd.ExecuteReader())
//    {                    
//        reader.Read();
//        DateTime date = reader.GetDate(0);
//        Console.WriteLine("Teradata Database DATE is {0}", date);
//    }
//}