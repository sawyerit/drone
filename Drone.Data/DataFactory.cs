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

namespace Drone.Data
{
    public static class DataFactory
    {

        #region Database Helper Methods

        public static DataSet GetDataSetByName(string StoredProcedureName, params object[] paramlist)
        {
            ProcedureDefinition oProc = StoredProcedures.GetProcedureDefinition(StoredProcedureName);
            SQLUtil utils = new SQLUtil();
            DataSet ds = new DataSet();

            if (!Object.Equals(paramlist, null))
            {
                foreach (KeyValuePair<string, object> param in paramlist)
                {
                    utils.AddParameter(param.Key, param.Value);
                }
            }

            ds = utils.ExecuteDataSet(oProc.FullyQualifiedCommand, oProc.Connection);
            if (Object.Equals(ds, null) || ds.Tables.Count <= 0) ds = null;

            return ds;
        }

        public static DataTable GetDataTableByName(string StoredProcedureName, params object[] paramlist)
        {
            DataTable dt = null;

            DataSet ds = GetDataSetByName(StoredProcedureName, paramlist);
            if (!Object.Equals(ds, null) && ds.Tables.Count > 0) dt = ds.Tables[0];

            return dt;
        }

        public static void ExecuteNonQuery(string StoredProcedureName, params object[] paramlist)
        {
            ProcedureDefinition oProc = StoredProcedures.GetProcedureDefinition(StoredProcedureName);
            SQLUtil utils = new SQLUtil();

            if (!Object.Equals(paramlist, null))
            {
                foreach (KeyValuePair<string, object> param in paramlist)
                {
                    utils.AddParameter(param.Key, param.Value);
                }
            }

            utils.ExecuteNonQuery(oProc.FullyQualifiedCommand, oProc.Connection);
        }

        public static void ExecuteNonQuery(string StoredProcedureName, List<KeyValuePair<string, object>> paramlist)
        {
            ProcedureDefinition oProc = StoredProcedures.GetProcedureDefinition(StoredProcedureName);
            SQLUtil utils = new SQLUtil();

            if (!Object.Equals(paramlist, null))
            {
                foreach (KeyValuePair<string, object> param in paramlist)
                {
                    utils.AddParameter(param.Key, param.Value);
                }
            }

            utils.ExecuteNonQuery(oProc.FullyQualifiedCommand, oProc.Connection);
        }

        public static string GetMD5Hash(string input)
        {
            byte[] dataBytes = null;
            using (MD5 md5 = MD5.Create())
            {
                dataBytes = md5.ComputeHash(Encoding.Default.GetBytes(input));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte dataByte in dataBytes) sb.AppendFormat("{0:x2}", dataByte);
            return sb.ToString();
        }

        /// <summary>
        /// Executes storedprocedure with a datatable as a parameter (for bulk merges)
        /// </summary>
        /// <param name="procname"></param>
        /// <param name="dt"></param>
        public static void ExecuteNonQueryTable(string procname, DataTable dt)
        {
            ProcedureDefinition oProc = StoredProcedures.GetProcedureDefinition(procname);

            using (SqlConnection con = new SqlConnection(GetConnection(oProc)))
            {
                try
                {
                    con.Open();

                    SqlCommand sqlCmd = new SqlCommand(oProc.FullyQualifiedCommand, con);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter tvpParam = sqlCmd.Parameters.AddWithValue("@TableVariable", dt);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        private static string GetConnection(ProcedureDefinition proc)
        {
            netConnect.Info info = new netConnect.Info();
            string DSNName = proc.Connection; //"BigM1DMStagingRW";
            string setting = ConfigurationManager.AppSettings["Environment"];
            if (string.IsNullOrEmpty(setting))
                setting = string.Empty;
            else
            {
                if (!setting.EndsWith("."))
                    setting += ".";
            }
            string sAppName = ConfigurationManager.AppSettings["NetConnect.ApplicationName"];
            string sCertName = ConfigurationManager.AppSettings[setting + "NetConnect." + DSNName + ".CertificateName"];
            if (sCertName == null)
            {
                sCertName = ConfigurationManager.AppSettings["NetConnect." + DSNName + ".CertificateName"];
                if (sCertName == null)
                {
                    sCertName = ConfigurationManager.AppSettings[setting + "NetConnect.CertificateName"];
                    if (sCertName == null)
                        sCertName = ConfigurationManager.AppSettings["NetConnect.CertificateName"];
                }
            }
            return info.Get(DSNName, sAppName, sCertName, netConnect.ConnectTypeEnum.CONNECT_TYPE_NET);
        }

        #endregion Database Helper Methods

        #region MongoDB Methods

        /// <summary>
        /// Gets n number of documents from the collection and marks them with a LastCrawlDate
        /// There is no transaction or locking except at the document level, so this only works
        /// with one instance of the crawler running against this dataset.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetDocumentsFromCollection(int count, string collection)
        {
            List<BsonDocument> docs = new List<BsonDocument>();
            List<BsonValue> fetchedIds = new List<BsonValue>();
            //Fetch using the query needed with sort, limiting the result set to my batch size
            //docs = coll.FindAllAs<BsonDocument>().SetSortOrder(SortBy.Ascending("LastCrawlDate")).SetLimit(count).ToList();                       
           
            
            docs = BSONHelper.GetCursorByJsonString<BsonDocument>("{$query:{}, $orderby: {LastCrawlDate:1}}", collection).SetLimit(count).ToList();            
            docs.ForEach(d => fetchedIds.Add(d["_id"]));
                       

            //Update with a clause to ensure the Id is IN the list returned in first step and set the lastcrawl date
            MongoCollection coll = BSONHelper.GetCollection(collection);
            coll.Update(Query.In("_id", fetchedIds), Update.Set("LastCrawlDate", DateTime.Now), UpdateFlags.Multi);

            return docs;
        }

        #endregion
    }

    //netConnect.Info info = new netConnect.Info();
    //string DSNName = "BigDWGDReports";
    //string setting = ConfigurationManager.AppSettings["Environment"];
    //if (string.IsNullOrEmpty(setting))
    //  setting = string.Empty;
    //else
    //{
    //  if (!setting.EndsWith("."))
    //    setting += ".";
    //}
    //string sAppName = ConfigurationManager.AppSettings["NetConnect.ApplicationName"];
    //string sCertName = ConfigurationManager.AppSettings[setting + "NetConnect." + DSNName + ".CertificateName"];
    //if (sCertName == null)
    //{
    //  sCertName = ConfigurationManager.AppSettings["NetConnect." + DSNName + ".CertificateName"];
    //  if (sCertName == null)
    //  {
    //    sCertName = ConfigurationManager.AppSettings[setting + "NetConnect.CertificateName"];
    //    if (sCertName == null)
    //      sCertName = ConfigurationManager.AppSettings["NetConnect.CertificateName"];
    //  }
    //}
    //string strConnection = info.Get(DSNName, sAppName, sCertName, netConnect.ConnectTypeEnum.CONNECT_TYPE_NET);		
}