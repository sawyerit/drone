using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.ServiceProcess;
using Drone.Data;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.WebAPI.Interfaces;
using System.Linq;
using System.Web.Script.Serialization;

namespace Drone.WebAPI.Services
{
    public class MiscService : BaseService, IMiscService
    {
        public List<Dictionary<string, object>> CompareQueries(string q1, string q2, string username)
        {
            string insertQuery = @"INSERT EXPLAIN INTO QCD AS {0} {1}";
            Random r = new Random();
            DataSet ds = new DataSet();
            int q1Result = 0; int q2Result = 0;
            bool q1set = false; bool q2set = false;
            string uniqID1 = string.Empty; string uniqID2 = string.Empty;

            try
            {
                username = username.Split('\\')[1];

                if (!String.IsNullOrEmpty(q1))
                {
                    uniqID1 = string.Format("qry1_{0}_{1}",username, r.Next().ToString()); //like this for logging
                    q1Result = TDDataFactory.ExecuteScalar<int>(string.Format(insertQuery, uniqID1, q1));
                    q1set = true;
                }
                if (!String.IsNullOrEmpty(q2))
                {
                    uniqID2 = string.Format("qry1_{0}_{1}", username, r.Next().ToString());
                    q2Result = TDDataFactory.ExecuteScalar<int>(string.Format(insertQuery, uniqID2, q2));
                    q2set = true;
                }

//                string query = @"SELECT qv.QueryID,qv.TotalCost,SUM_CPU,SUM_IO 
//                            from QCD.QueryView qv 
//                            JOIN (SELECT QueryID,SUM(EstCPUCost) AS SUM_CPU,SUM(EstIOCost) AS SUM_IO,SUM(EstHRCost) AS SUM_HR,SUM(EstNetworkCost) AS SUM_NET 
//                                    FROM QCD.QueryStepsView WHERE stepnum > 0 AND ParallelStepNum = 0 GROUP BY QueryID ) qsv 
//                                    ON qv.QueryID=qsv.QueryID 
//                                    WHERE qv.QueryID IN (" + q1Result + "," + q2Result + ") ORDER BY qv.QueryID;";

                string query = @"SELECT qv.QueryID,
                                cast((qv.TotalCost (FORMAT 'GZZZZZZZZZZZZZZZ9D99')) AS CHAR(18)) AS totalCost,
                                cast((SUM_CPU (FORMAT 'GZZZZZZZZZZZZZZZ9D99')) AS CHAR(18)) AS cpuCost,
                                cast((SUM_IO (FORMAT 'GZZZZZZZZZZZZZZZ9D99')) AS CHAR(18)) AS ioCost
                            from QCD.QueryView qv
                            JOIN (SELECT QueryID,SUM(EstCPUCost) AS SUM_CPU,SUM(EstIOCost) AS SUM_IO,SUM(EstHRCost) AS SUM_HR,SUM(EstNetworkCost) AS SUM_NET
                                FROM QCD.QueryStepsView 
                                WHERE stepnum > 0
                                AND ParallelStepNum = 0
                                GROUP BY QueryID
                                ) qsv
                            ON qv.QueryID=qsv.QueryID
                            WHERE qv.QueryID IN (" + q1Result + "," + q2Result + ") ORDER BY qv.QueryID;";


                ds = TDDataFactory.ExecuteDataSet(query);
            }
            catch (Exception e)
            {
                string lastQuery = string.Empty;
                if (e.Message.ToLower().Contains("teradata"))
                {
                    lastQuery = q2set ? "Select from QCD" : q1set ? "Query 2" : "Query 1";
                    ds.Tables.Add(new DataTable());
                    ds.Tables[0].Columns.Add("error");
                    ds.Tables[0].Rows.Add(lastQuery + " " + e.Message);
                } else
                {
                    ExceptionExtensions.LogError(e, "Drone.WebApi.Services.MiscService.CompareQueries","User: " + username);;
                    throw new Exception("A server exception occured and an error notification has been sent. <br/>Error: " + e.Message);
                }
            }
            finally
            {
                ExceptionExtensions.LogInformation("MiscService.CompareQueries", "UniqueID1: " + uniqID1 + "queryID1: " + q1Result);
                ExceptionExtensions.LogInformation("MiscService.CompareQueries", "UniqueID2: " + uniqID2 + "queryID2: " + q2Result);

                TDDataFactory.ExecuteNonQuery(string.Format("EXECUTE QCD.DelQid_V3({0})", q1Result));
                TDDataFactory.ExecuteNonQuery(string.Format("EXECUTE QCD.DelQid_V3({0})", q2Result));
            }

            return DataTableToList(ds.Tables[0]);
        }

        public static List<Dictionary<string, object>> DataTableToList(DataTable table)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }

            return list;
        }
    }
}