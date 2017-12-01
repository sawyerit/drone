using Drone.API.Facebook;
using Drone.API.Twitter;
using Drone.API.YouTube;
using Drone.Entities.Crunchbase;
using Drone.Entities.Facebook;
using Drone.Entities.MarketShare;
using Drone.Entities.Portfolio;
using Drone.Entities.Twitter.v11;
using Drone.Entities.WebAPI;
using Drone.Entities.YouTube;
using Drone.Facebook.Components;
using Drone.Facebook.Datasources;
using Drone.Scheduler;
using Drone.Scheduler.Datasources;
using Drone.Shared;
using Drone.Twitter.Datasources;
using Drone.WebAPI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Drone.Tests
{
    [TestClass]
    public class WebAPITests
    {
        HttpWebRequest requestGet;
        HttpWebRequest requestPost;
        ChannelManager _cManager = null;

        int testId = 1;

        #region Get tests

        [TestMethod]
        public void GetCrunchbaseFromRest()
        {
            string result = string.Empty;
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/crunchbase");
            requestGet.UseDefaultCredentials = true;
            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetMarketShareFromRest()
        {
            //returns a list of marketshare data for a particular domain id
            string result = string.Empty;
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/marketshare/12");
            requestGet.UseDefaultCredentials = true;
            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<MarketShareDataType> md = new JavaScriptSerializer().Deserialize<List<MarketShareDataType>>(result);
            Assert.IsNotNull(md);
            Assert.AreEqual(12, md[0].DomainId);
        }

        [TestMethod]
        public void GetDomainFromRest()
        {
            //must make sure the data is not already processed for mask 4 for the test to pass
            string result = string.Empty;
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/domains/lookup/testdomain");
            requestGet.UseDefaultCredentials = true;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            Domain domain = new JavaScriptSerializer().Deserialize<Domain>(result);

            Assert.IsNotNull(domain);
        }

        [TestMethod]
        public void GetDomainsFromRest()
        {
            //must make sure the data is not already processed for mask 4 for the test to pass
            string result = string.Empty;
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/domains?count=5&mask=4");
            requestGet.UseDefaultCredentials = true;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<Domain> domainList = new JavaScriptSerializer().Deserialize<List<Domain>>(result);

            Assert.IsNotNull(domainList);
            Assert.AreEqual(5, domainList.Count());
        }

        [TestMethod]
        public void GetCommonCompetitorsFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/common/competitors");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<Competitor> compList = new JavaScriptSerializer().Deserialize<List<Competitor>>(result);

            Assert.IsNotNull(compList);
            Assert.AreEqual(33, compList.Count());
        }

        [TestMethod]
        public void GetFacebookAllPagesFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/facebook/");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<Page> pList = new JavaScriptSerializer().Deserialize<List<Page>>(result);

            Assert.IsNotNull(pList);
            Assert.AreNotEqual(0, pList.Count());
        }

        [TestMethod]
        public void GetFacebookPageByIDFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://p3pwsvcweb001/BIdata/api/facebook/godaddy");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            Page p = new JavaScriptSerializer().Deserialize<Page>(result);

            Assert.IsNotNull(p);
            Assert.AreEqual("godaddy", p.Id);
        }

        [TestMethod]
        public void GetYouTubeVideoByIDFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/youtube/12");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            ChannelVideo cv = new JavaScriptSerializer().Deserialize<ChannelVideo>(result);

            Assert.IsNotNull(cv);
            Assert.AreEqual(cv.ChannelVideoID, "12");
        }

        [TestMethod]
        public void GetYouTubeChannelFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/youtube/channel/godaddy");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            Channel c = new JavaScriptSerializer().Deserialize<Channel>(result);

            Assert.IsNotNull(c);
            Assert.AreEqual(c.Name, "godaddy");
            Assert.AreNotEqual(0, c.TotalSubscribers);
        }

        [TestMethod]
        public void GetTwitterMentionsFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/twitter/mentions");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<KeywordStatus> statusList = new JavaScriptSerializer().Deserialize<List<KeywordStatus>>(result);

            Assert.IsNotNull(statusList);
            Assert.AreNotEqual(0, statusList.Count());
            Assert.AreEqual(100, statusList[0].StatusList.Count());
        }

        [TestMethod]
        public void GetTwitterMentionsPagedFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/twitter/mentions?page=5");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            List<KeywordStatus> statusList = new JavaScriptSerializer().Deserialize<List<KeywordStatus>>(result);

            Assert.IsNotNull(statusList);
            Assert.AreNotEqual(0, statusList.Count());
            Assert.AreEqual(10, statusList[0].StatusList.Count());
        }

        [TestMethod]
        public void GetTwitterMentionByIdFromRest()
        {
            requestGet = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/twitter/mentions/12");
            requestGet.Method = "GET";
            requestGet.ContentType = "application/json";
            requestGet.UseDefaultCredentials = true;

            string result = string.Empty;

            // Get response  
            using (HttpWebResponse response = requestGet.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            Drone.Entities.Twitter.v11.Status status = new JavaScriptSerializer().Deserialize<Drone.Entities.Twitter.v11.Status>(result);

            Assert.IsNotNull(status);
            Assert.AreEqual(12, status.id);
        }



        #endregion

        #region Post Tests


        [TestMethod]
        public void PostCrunchbaseToRest()
        {
            string result = string.Empty;
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/crunchbase");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;
            //requestPost.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            //post data
            CompanyRoot cr = new CompanyRoot();
            cr.permalink = "test-company";
            cr.name = "test company";
            cr.homepage_url = "testcompany.com";
            cr.records = new Records { CertificateType = "None", SSLIssuer = "None", DNSHost = "GoDaddy", Registrar = "GoDaddy", EmailHost = "None", WebHost = "1and1" };

            string requestData = new JavaScriptSerializer().Serialize(cr);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

        }

        [TestMethod]
        public void PostFacebookPageToRest()
        {
            string result = string.Empty;
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/facebook/page");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;

            Entities.Facebook.Page page = new Entities.Facebook.Page { Id = "14240968757", Likes = 152, Name = "Test Facebook Page", Website = "Test Page Url", Category = "Test Category", Talking_About_Count = 12 };

            string requestData = new JavaScriptSerializer().Serialize(page);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public void PostFacebookCountryDemographicToRest()
        {
            string result = string.Empty;
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/facebook/country");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;

            Demographic<Country> country = GetCountryDemographic();

            string requestData = new JavaScriptSerializer().Serialize(country);
            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

            response.Close();
        }

        [TestMethod]
        public void PostMarketShareToRest()
        {
            string result = string.Empty;
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/bidataapi/api/marketshare");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;

            MarketShareDataType msdt = new MarketShareDataType { BitMaskId = 4, TypeId = 3, DomainId = 73557, Value = "Go Daddy", SampleDate = DateTime.Now.ToString() };

            string requestData = new JavaScriptSerializer().Serialize(msdt);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public void PostManyMarketShareToRest()
        {
            for (int i = 0; i < 1000; i++)
            {
                string result = string.Empty;
                requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/marketshare");
                requestPost.Method = "POST";
                requestPost.ContentType = "application/json";
                requestPost.UseDefaultCredentials = true;

                MarketShareDataType msdt = new MarketShareDataType { BitMaskId = 4, TypeId = 3, DomainId = i, Value = "Go Daddy", SampleDate = "02/07/2013" };

                string requestData = new JavaScriptSerializer().Serialize(msdt);

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                using (Stream dataStream = requestPost.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);

                HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            }
        }

        [TestMethod]
        public void PostGoDaddyChannelYouTubeToRest()
        {
            string result = string.Empty;//http://localhost/BIdataApi/api/youtube
            requestPost = (HttpWebRequest)WebRequest.Create("http://p3pwsvcweb001/bidata/api/youtube");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;
            //requestPost.PreAuthenticate = true;

            _cManager = new ChannelManager(new KeyValuePair<int, string>(1, "GoDaddy"), "Drone Processor", "AI39si7H3JsgcnrDNQ_-_NVMklAztUBMREtgH3pdiIb0iX9ASor__Nw5q2tT-0V1H8gnnVZyFsrPQpUmBcdAS6HWswa4UNaUyw");
            Channel c = _cManager.GetUserChannel();

            string requestData = new JavaScriptSerializer().Serialize(c);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public void PostTwitterMentionsToRest()
        {
            string result = string.Empty;
            //http://localhost/BIDataAPI/api/twitter/mentions
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/twitter/mentions");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;

            TweetManager tm = new TweetManager();
            KeywordStatus ks = new KeywordStatus();
            Twitter.Components.Twitter t = new Twitter.Components.Twitter();
            ks.KeywordId = 7;
            ks.StatusList = tm.GetTweetsByQuery(20, Uri.EscapeDataString("\"go daddy\" OR godaddy OR go-daddy"), Twitter.Utility.GetOAuthToken(t.Xml));

            string requestData = new JavaScriptSerializer().Serialize(ks);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

        }

        [TestMethod]
        public void PostTwitterUsersToRest()
        {
            string result = string.Empty;
            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/twitter/users");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/json";
            requestPost.UseDefaultCredentials = true;

            List<User> _twitterUserList = new List<User>();
            TwitterUserDataSource dataSource = new TwitterUserDataSource();
            UserManager um = new UserManager();
            Twitter.Components.Twitter t = new Twitter.Components.Twitter();

            foreach (Competitor comp in dataSource.GetCompetitorAccounts())
            {
                _twitterUserList.Add(um.GetTwitterUserInfo(comp.TwitterID, Twitter.Utility.GetOAuthToken(t.Xml)));
            }

            string requestData = new JavaScriptSerializer().Serialize(_twitterUserList);

            byte[] data = Encoding.UTF8.GetBytes(requestData);

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

        }

        [TestMethod]
        public void PostManyCrunchbase()
        {
            //post data
            for (int i = 0; i < 30; i++)
            {
                string result = string.Empty;
                requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/crunchbase");
                requestPost.Method = "POST";
                requestPost.ContentType = "application/json";
                requestPost.UseDefaultCredentials = true;

                CompanyRoot cr = new CompanyRoot();
                cr.permalink = "test-company" + i.ToString();
                cr.name = "test company" + i.ToString();
                cr.homepage_url = "testcompany.com";
                cr.records = new Records { CertificateType = "None", SSLIssuer = "None", DNSHost = "GoDaddy", Registrar = "GoDaddy", EmailHost = "None", WebHost = "GoDaddy" };

                string requestData = new JavaScriptSerializer().Serialize(cr);

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                using (Stream dataStream = requestPost.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);

                HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

        }

        [TestMethod]
        public void PostmanyFacebookPageToRest()
        {
            for (int i = 0; i < 30; i++)
            {
                string result = string.Empty;
                requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/facebook/page");
                requestPost.Method = "POST";
                requestPost.ContentType = "application/json";
                requestPost.UseDefaultCredentials = true;

                Entities.Facebook.Page page = new Entities.Facebook.Page { Id = i.ToString(), Likes = 152, Name = "Test Facebook Page", Website = "Test Page Url", Category = "Test Category", Talking_About_Count = 12 };

                string requestData = new JavaScriptSerializer().Serialize(page);

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                using (Stream dataStream = requestPost.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);

                HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

        }

        [TestMethod]
        public void PostManyPortfolioToRest()
        {
            for (int i = 0; i < 100; i++)
            {
                string result = string.Empty;
                requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/portfolio");
                requestPost.Method = "POST";
                requestPost.ContentType = "application/json";
                requestPost.UseDefaultCredentials = true;

                PortfolioDataType msdt = new PortfolioDataType { TypeId = 3, rptGDDomainsId = i, Value = "Go Daddy", ShopperID = "sid" + i.ToString() };

                string requestData = new JavaScriptSerializer().Serialize(msdt);

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                using (Stream dataStream = requestPost.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);

                HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            }
        }

        [TestMethod]
        public void PostJSONToRest()
        {
            for (int i = 0; i < 100; i++)
            {
                string result = string.Empty;
                requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIdataApi/api/common/domaincomplete");
                requestPost.Method = "POST";
                requestPost.ContentType = "application/json";
                requestPost.UseDefaultCredentials = true;

                string requestData = string.Format("{{ Name : \"{0}\", DomainComplete : {1}, Pages : {2} }}", "testdomain.com", 59.88, 12);

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                using (Stream dataStream = requestPost.GetRequestStream())
                    dataStream.Write(data, 0, data.Length);

                HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
        }

        [TestMethod]
        public void PostTDCompareToRest()
        {
            string result = string.Empty;
            //post data
            StringBuilder requestData = new StringBuilder();
            requestData.Append("q1text=" + HttpUtility.UrlEncode("testquery1") + "&");
            requestData.Append("q2text=" + HttpUtility.UrlEncode("testquery2") + "&");
            requestData.Append("usertext=" + HttpUtility.UrlEncode("jomax\\ssawyer"));

            requestPost = (HttpWebRequest)WebRequest.Create("http://localhost/BIDataAPI/api/misc/tdcompare");
            requestPost.Method = "POST";
            requestPost.ContentType = "application/x-www-form-urlencoded";
            requestPost.ContentLength = requestData.Length;
            requestPost.UseDefaultCredentials = true;
                        
            byte[] data = Encoding.ASCII.GetBytes(requestData.ToString());

            using (Stream dataStream = requestPost.GetRequestStream())
                dataStream.Write(data, 0, data.Length);

            HttpWebResponse response = requestPost.GetResponse() as HttpWebResponse;
            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

        }

        #endregion

        [TestMethod]
        [Ignore]
        public void WebAPI_MonitorService_Test()
        {
            Entities.WebAPI.Status s = MonitorService.GetStatus();
        }

        [TestMethod]
        [Ignore]
        public void WebAPI_MonitorService_DisableService()
        {
            //MSCamS64.exe
            MonitorService.RunCommand("stop", "MSCamSvc");
            MonitorService.RunCommand("start", "MSCamSvc");
        }

        [TestMethod]
        [Ignore]
        public void WebAPI_RService_Test()
        {
            RService r = new RService();
            XElement x = r.Get("GlobalWeekly.r", "WeekEnding|Business Registrations|US|2013-01-09|2013-07-09");
            
        }

        [TestMethod]
        [Ignore]
        public void WebAPI_DomainServiceGet_Test()
        {
            DomainsService ds = new DomainsService();
            List<Domain> dmns = ds.Get(10);

            Assert.AreEqual(10, dmns.Count);
        }

        [TestMethod]
        [Ignore]
        public void WebAPI_DomainServiceGetFromMongo_Test()
        {
            DomainsService ds = new DomainsService();
            List<Domain> dmns = ds.GetFromMongo(5, "TestSource");

            Assert.AreEqual(10, dmns.Count);
        }

        [TestMethod]
        public void WebAPI_CommonService_PeekQueueTest()
        {
            CommonService cs = new CommonService();
            var list = cs.PeekQueue(2);

        }

        [TestMethod]
        public void WebAPI_MiscService_TeraDateCompareTest()
        {
            MiscService ms = new MiscService();
            var result = ms.CompareQueries("select '1'", "select '2'", @"jomax\ssawyer");
        }

        #region helper

        private static Demographic<Country> GetCountryDemographic()
        {
            FacebookDataComponent fdc = new FacebookDataComponent();
            FacebookFanInfo ffi = new FacebookFanInfo(new FacebookDataSource());

            string accountId = XMLUtility.GetTextFromAccountNode(ffi.Xml, "id");
            string accessToken = XMLUtility.GetTextFromAccountNode(ffi.Xml, "accesstoken");
            Graph _graph = new Graph();
            Demographic<Country> country = _graph.GetFanDemographics<Demographic<Country>>(accountId, accessToken, "page_fans_country");
            return country;
        }

        #endregion
    }
}
