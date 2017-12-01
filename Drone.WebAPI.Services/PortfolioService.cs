using Drone.Data;
using Drone.Entities.MarketShare;
using Drone.Entities.Portfolio;
using Drone.Shared;
using Drone.WebAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Drone.WebAPI.Services
{
    public class PortfolioService : BaseService, IPortfolioService
    {
        public PortfolioDataType Create(PortfolioDataType value)
        {
            try
            {
                value.Value = Regex.Replace(value.Value, "[^\x20-\x7E]", String.Empty);
                _queueManager.AddToQueue(Utility.SerializeToXMLString<PortfolioDataType>(value), "Portfolio DomainId " + value.rptGDDomainsId.ToString());
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "PortfolioService.Create", "value: " + value.ToString());
            }

            return value;
        }

        public List<PortfolioFullDataType> Get(string id)
        {
            List<PortfolioFullDataType> portList = new List<PortfolioFullDataType>();
            DataSet ds;

            int n;
            if (int.TryParse(id, out n))
            {
                //get domain by shopperid
                ds = DataFactory.GetDataSetByName("PortfolioDomainsGetByShopper", new KeyValuePair<string, object>("@ShopperID", id));
            }
            else
            {
                //get domain by domainname
                ds = DataFactory.GetDataSetByName("PortfolioDomainsGetByName", new KeyValuePair<string, object>("@DomainName", id));
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                ConvertToTypedList(portList, ds);
            }

            return portList;
        }

        private void ConvertToTypedList(List<PortfolioFullDataType> portList, DataSet ds)
        {
            PortfolioFullDataType d = null;
            string curDomain = string.Empty;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string dmnName = dr["DomainName"].ToString().ToLowerInvariant();

                if (curDomain != dmnName)
                {
                    curDomain = dmnName;
                    d = new PortfolioFullDataType();
                    portList.Add(d);

                    if (dr.Table.Columns.Contains("ID"))
                        d.rptGDDomainsId = dr["ID"].ToString().ConvertStringToInt(0);

                    if (dr.Table.Columns.Contains("DomainID"))
                        d.DomainID = dr["DomainID"].ToString().ConvertStringToInt(0);

                    if (dr.Table.Columns.Contains("ShopperID"))
                        d.ShopperID = dr["ShopperID"].ToString().ConvertStringToInt(0);
                    d.DomainName = dmnName;

                    if (dr.Table.Columns.Contains("LastCrawlDate"))
                        d.LastCrawlDate = Convert.ToDateTime(dr["LastCrawlDate"]);

                    if (dr.Table.Columns.Contains("PrivateLabelID"))
                        d.PrivateLabelID = dr["PrivateLabelID"].ToString().ConvertStringToInt(0);

                    d.Attributes = new Dictionary<string, string>();
                    d.Verticals = new Dictionary<string, string>();
                    d.Social = new Dictionary<string, string>();
                }

                int type = dr["TypeID"].ToString().ConvertStringToInt(0);
                if (type == 14 || type == 15)
                {
                    string[] attrs = dr["Value"].ToString().Split(',');
                    foreach (var keyVal in attrs)
                    {
                        string[] pair = keyVal.Split('|');
                        if (pair.Length > 1)
                        {
                            if (type == 14)
                                d.Verticals.Add(pair[0], pair[1]);

                            if (type == 15)
                                d.Social.Add(pair[0], pair[1]);
                        }
                    }
                }
                else
                {
                    d.Attributes.Add(GetAttributeName(type), dr["Value"].ToString());
                }
            }
        }

        public void GetAllPortfolioByType(string type)
        {
            DataSet ds = new DataSet();

            if (!Object.Equals(null, type))
            {
                string countsql = "select count(1) as cnt from gdwebmarketshare.dbo.rptGDDomainAttributes a with (nolock) JOIN gdwebmarketshare.dbo.rptGDDomains b ON a.rptGdDomainsID = b.ID where a.TypeID = " + type + " AND b.isActive = 1";
                DataSet countDS = FillDS(countsql);

                string sql = string.Empty;
                int numRowsAtATime = 100000;
                Double numItems = Convert.ToDouble(countDS.Tables[0].Rows[0]["cnt"].ToString());
                double numCalls = Math.Ceiling(numItems / numRowsAtATime);
                //calculate # of calls. and call

                for (int i = 0; i < numCalls; i++)
                {
                    List<PortfolioFullDataType> portList = new List<PortfolioFullDataType>();
                    sql = "select a.TypeID, a.Value, b.DomainID, b.DomainName from gdwebmarketshare.dbo.rptGDDomainAttributes a with (nolock) JOIN gdwebmarketshare.dbo.rptGDDomains b ON a.rptGdDomainsID = b.ID where a.TypeID = " + type + " AND b.isActive = 1 order by b.LastCrawlDate desc OFFSET " + i * numRowsAtATime + " ROWS FETCH NEXT " + numRowsAtATime + " ROWS ONLY";
                    DataSet jsonDS = FillDS(sql);

                    //convert all results to typed list
                    ConvertToTypedList(portList, jsonDS);

                    //write to json file
                    string logFile = "Logs\\AllVerticalDomains" + i + ".json";
                    try
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        js.MaxJsonLength = Int32.MaxValue;
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true))
                        {
                            file.WriteLine(js.Serialize(portList));
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        private static DataSet FillDS(string sql)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300000;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public List<PortfolioDataType> GetAll()
        {
            return new List<PortfolioDataType>();
        }

        public List<PortfolioDataType> GetPaged(int page, int count)
        {
            return new List<PortfolioDataType>();
        }


        #region private helpers

        private string GetAttributeName(int type)
        {
            return ((MarketShareDataTypeEnum)type).ToString();
        }

        private static string GetConnection()
        {
            netConnect.Info info = new netConnect.Info();
            string DSNName = "BigM1DMStaging";
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
        #endregion
    }
}