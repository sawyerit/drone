using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Drone.Data;
using Drone.Entities.WebAPI;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
    public class DomainsService : BaseService, IDomainsService
    {
        //portfolio get
        public List<Domain> Get(int count)
        {
            List<Domain> domainList = new List<Domain>();

            try
            {
                DataTable dt = DataFactory.GetDataTableByName("PortfolioDomainsGet", new KeyValuePair<string, object>("@rows", count));

                if (!Object.Equals(dt, null))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string domain = Utility.CleanUrl(row["DomainName"].ToString());
                        Dictionary<string, string> domainAttrs = new Dictionary<string, string>();
                        domainList.Add(new Domain
                        {
                            DomainId = Convert.ToInt32(row["ID"]),
                            ShopperID = row["ShopperID"].ToString(),
                            Uri = new Uri("http://" + domain),
                            DomainName = domain,
                            DomainAttributes = domainAttrs
                        });
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("deadlocked"))
                {
                    Thread.Sleep(1000);
                    Get(count);
                }
                else
                {
                    ExceptionExtensions.LogError(e, "DomainsService.Get");
                }
            }

            return domainList;
        }

        //marketshare get
        public List<Domain> Get(int count, int mask)
        {
            List<Domain> domainList = new List<Domain>();

            try
            {
                DataTable dt = DataFactory.GetDataTableByName("MarketShareDomainsByTypeDate"
                                                                                                            , new KeyValuePair<string, object>("@Rows", count)
                                                                                                            , new KeyValuePair<string, object>("@BitMask", mask));

                if (!Object.Equals(dt, null) && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Dictionary<string, string> domainAttrs = new Dictionary<string, string>();
                        domainAttrs.Add("SampleDate", row["SampleDate"].ToString());
                        domainList.Add(new Domain { DomainId = Convert.ToInt32(row["DomainID"]), Uri = new Uri("http://" + Utility.CleanUrl(row["Domain"].ToString())), DomainAttributes = domainAttrs });
                    }
                }
                else
                {
                    //If there are 0 records, call general clean up proc.  Next hourly run will pickup any cleaned items.
                    DataFactory.ExecuteNonQuery("MarketShareCleanUp");
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("deadlocked"))
                {
                    Thread.Sleep(1000);
                    Get(count, mask);
                }
                else
                {
                    ExceptionExtensions.LogError(e, "MarketShareService.Get", "bitmaskid: " + mask);
                }
            }

            return domainList;
        }

        public Domain Create(Domain domain)
        {
            //TODO: Adds a domain to the list of portfolio domains? Prob don't want to do this.
            return domain;
        }

        public Domain LookupDomain(string domain)
        {
            //TODO: this is for passing in a domain that doesn't exist, and performing a full lookup of attributes on that domain
            //run a seperate crawler process for gathering info on this domain and sending it back to the user (as well as the database??)
            throw new NotImplementedException();
        }

        public List<Domain> GetFromMongo(int count, string sourceCollection)
        {
            List<Domain> domainList = new List<Domain>();

            try
            {
                foreach (var doc in DataFactory.GetDocumentsFromCollection(count, sourceCollection))
                {
                    string domain = Utility.CleanUrl(doc.GetValue("DomainName").AsString);
                    Dictionary<string, string> domainAttrs = new Dictionary<string, string>();
                    domainList.Add(new Domain
                    {
                        DomainId = doc.GetValue("rptGdDomainsID").AsInt32,
                        DomainName = domain,
                        ShopperID = doc.GetValue("ShopperID", 0).ToString(),
                        Uri = new Uri("http://" + domain),
                        DomainAttributes = domainAttrs
                    });
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "DomainsService.GetFromMongo");
            }

            return domainList;


        }
    }
}