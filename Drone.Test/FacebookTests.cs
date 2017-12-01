using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.Facebook.Components;
using Drone.Facebook.Datasources;
using Drone.Entities.Facebook;
using System.IO;
using Drone.Data.Queue;
using Drone.API.Facebook;
using Drone.Shared;
using System.Web.Script.Serialization;
using System.Globalization;

namespace Drone.Test
{
    [TestClass]
    public class FacebookTests
    {
        public FacebookFanInfo FanInfo { get; set; }
        public QueueManager QManager { get; set; }
        private Graph _graph;

        [TestInitialize]
        public void FacebookTest_Setup()
        {
            Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "socialmedia");

            FanInfo = new FacebookFanInfo(new FacebookDataSource());
            _graph = new Graph();
        }

        [TestMethod]
        public void FacebookFanInfo_SetNextRunByNode()
        {
            FanInfo.SetNextRunIntervalByNode(FanInfo.ProcessorName, FanInfo.Context);

            Assert.AreEqual(FanInfo.Context.NextRun, DateTime.Today.AddDays(1));
        }

        [TestMethod]
        public void Facebook_GetCompanyPageInfo()
        {
            Page fpo = FacebookFanInfo.GetPageInfoByCompany(8749090685);

            Assert.AreNotEqual(0, fpo.Likes);
            Assert.AreEqual("Dotster", fpo.Name);
        }

        [TestMethod]
        public void Facebook_GetCompanyPageInfoAndSave()
        {
            Page fpo = FacebookFanInfo.GetPageInfoByCompany(8749090685);

            Assert.AreNotEqual(0, fpo.Likes);
            Assert.AreEqual("GoDaddy.com", fpo.Name);

            FacebookDataComponent fdc = new FacebookDataComponent();
            fdc.FBPage = fpo;

            FanInfo.DroneDataSource.Process(fdc);

            //Assert.IsTrue(ReadLineFromFile().Contains("GoDaddy.com"));

        }

        [TestMethod]
        public void Facebook_GetCompanyPageInfoAndInsertQueue()
        {
            FacebookFanInfo fg = new FacebookFanInfo(); //MEF objects are created with the QM passed in... exposed for testing. 
            Page fpo = FacebookFanInfo.GetPageInfoByCompany(80504973291);

            Assert.AreNotEqual(0, fpo.Likes);
            Assert.AreEqual("Dotster", fpo.Name);

            //fg.Context.MSMQManager.AddToQueue(fpo, fpo.Name);

            Assert.IsNotNull(fpo);
        }

        [TestMethod]
        public void Facebook_GetData()
        {
            FacebookFanInfo fg = new FacebookFanInfo();
            fg.GetData(fg.Context);

            Assert.IsNotNull(fg);
        }

        [TestMethod]
        public void Facebook_GetFanDemographics()
        {
            FacebookDataComponent fdc = new FacebookDataComponent();
            FacebookFanInfo ffi = new FacebookFanInfo(new FacebookTestDataSource());

            string accountId = XMLUtility.GetTextFromAccountNode(ffi.Xml, "id");
            string accessToken = XMLUtility.GetTextFromAccountNode(ffi.Xml, "accesstoken");

            Demographic<Country> country = _graph.GetFanDemographics<Demographic<Country>>(accountId, accessToken, "page_fans_country");
            Assert.IsNotNull(country.Data);
            Assert.AreNotEqual(0, country.Data.Count);
            Assert.IsNotNull(country.Data[0].Days);
            Assert.AreNotEqual(0, country.Data[0].Days.Count);
            Assert.IsNotNull(country.Data[0].Days[0].Country);
            Assert.AreNotEqual(0, country.Data[0].Days[0].Country.US);

            fdc.CountryDemographic = country;
            FanInfo.DroneDataSource.Process(fdc);


            Demographic<Locale> locale = _graph.GetFanDemographics<Demographic<Locale>>(accountId, accessToken, "page_fans_locale");
            Assert.IsNotNull(locale.Data);
            Assert.AreNotEqual(0, locale.Data.Count);
            Assert.IsNotNull(locale.Data[0].Days);
            Assert.AreNotEqual(0, locale.Data[0].Days.Count);
            Assert.IsNotNull(locale.Data[0].Days[0].Locale);
            Assert.AreNotEqual(0, locale.Data[0].Days[0].Locale.en_US);

            fdc = new FacebookDataComponent();
            fdc.LocaleDemographic = locale;
            FanInfo.DroneDataSource.Process(fdc);


            Demographic<Gender> gender = _graph.GetFanDemographics<Demographic<Gender>>(accountId, accessToken, "page_fans_gender_age");
            Assert.IsNotNull(gender.Data);
            Assert.AreNotEqual(0, gender.Data.Count);
            Assert.IsNotNull(gender.Data[0].Days);
            Assert.AreNotEqual(0, gender.Data[0].Days.Count);
            Assert.IsNotNull(gender.Data[0].Days[0].Gender);
            Assert.AreNotEqual(0, gender.Data[0].Days[0].Gender.M_25to34);

            fdc = new FacebookDataComponent();
            fdc.GenderDemographic = gender;
            FanInfo.DroneDataSource.Process(fdc);
        }

        [TestMethod]
        [Ignore]
        public void Facebook_GetFanDemographicsAndInsertQueue()
        {
            FacebookFanInfo fg = new FacebookFanInfo();

            string accountId = XMLUtility.GetTextFromAccountNode(fg.Xml, "id");
            string accessToken = XMLUtility.GetTextFromAccountNode(fg.Xml, "accesstoken");

            Demographic<Country> country = _graph.GetFanDemographics<Demographic<Country>>(accountId, accessToken, "page_fans_country");
            if (!Object.Equals(country, null))
            //fg.Context.MSMQManager.AddToQueue(country, "country");
            { }

            Demographic<Locale> locale = _graph.GetFanDemographics<Demographic<Locale>>(accountId, accessToken, "page_fans_locale");
            if (!Object.Equals(locale, null))
            //fg.Context.MSMQManager.AddToQueue(locale, "locale");
            { }

            Demographic<Gender> gender = _graph.GetFanDemographics<Demographic<Gender>>(accountId, accessToken, "page_fans_gender_age");
            if (!Object.Equals(gender, null))
            //fg.Context.MSMQManager.AddToQueue(gender, "gender");
            { }

            Assert.IsNotNull(fg);
        }


        [TestMethod]
        public void Facebook_Custom_GenderJSONSerializer()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DemographicJavaScriptConverter() });
            var dataObj = serializer.Deserialize<Demographic<Gender>>("{ \"data\": [ { \"id\": \"8749090685\", \"name\": \"page_fans_gender_age\", \"period\": \"lifetime\", \"values\": [ { \"value\": { \"M.25-34\": 24331, \"M.18-24\": 21211, \"F.13-17\": 1234 }, \"end_time\": \"2012-04-08\" } ], \"title\": \"Likes\", \"description\": \"Lifetime\" } ] }");

            Assert.AreEqual(24331, dataObj.Data[0].Days[0].Gender.M_25to34);
            Assert.AreEqual(21211, dataObj.Data[0].Days[0].Gender.M_18to24);
            Assert.AreEqual(1234, dataObj.Data[0].Days[0].Gender.F_13to17);

            var cdataObj = serializer.Deserialize<Demographic<Country>>("{ \"data\": [ { \"id\": \"8749090685\", \"name\": \"page_fans_country\", \"period\": \"lifetime\", \"values\": [ { \"value\": { \"US\": 66887, \"IN\": 10339 }, \"end_time\": \"2012-04-08\" } ], \"title\": \"Likes\", \"description\": \"Lifetime\" } ] }");

            Assert.AreEqual(10339, cdataObj.Data[0].Days[0].Country.IN);
            Assert.AreEqual(66887, cdataObj.Data[0].Days[0].Country.US);

            var ldataObj = serializer.Deserialize<Demographic<Locale>>("{ \"data\": [ { \"id\": \"8749090685\", \"name\": \"page_fans_country\", \"period\": \"lifetime\", \"values\": [ { \"value\": { \"ar_AR\": 872, \"bg_BG\": 189 }, \"end_time\": \"2012-04-08\" } ], \"title\": \"Likes\", \"description\": \"Lifetime\" } ] }");

            Assert.AreEqual(872, ldataObj.Data[0].Days[0].Locale.ar_AR);
            Assert.AreEqual(189, ldataObj.Data[0].Days[0].Locale.bg_BG);
        }

        [TestMethod]
        public void Facebook_ParseSocialMedia_Output()
        {

            Dictionary<string, SocialMediaData> socialEntries = new Dictionary<string, SocialMediaData>();

            string line;
            string filePath = Path.Combine(Utility.ComponentBaseFolder + "\\Logs", "Crawl_SocialMedia.rpt");

            StreamReader file = new StreamReader(filePath);

            while ((line = file.ReadLine()) != null)
            {
                if (!line.StartsWith("-"))
                {
                    try
                    {

                        SocialMediaData smd = new SocialMediaData();
                        if (line.Length > 255)
                        {
                            smd.Value = line.Substring(0, 255).Trim();
                            if (line.Length > 277)
                            {
                                smd.DomainID = Convert.ToInt32(line.Substring(257, 15));
                                if (line.Length > 406)
                                {
                                    smd.DomainName = line.Substring(278, 128).Trim();
                                    smd.ShopperID = Convert.ToInt32(line.Substring(407, line.Length - 407));
                                }
                            }
                        }

                        SocialMediaData cleaned = CleanSocialMediaValue(smd);
                        string key = cleaned.Value + cleaned.ShopperID.ToString();

                        if (!socialEntries.ContainsKey(key))
                        {
                            socialEntries.Add(key, cleaned);
                        }
                    }
                    catch (Exception e)
                    {
                        //skip line for now
                    }
                }
            }

            
            file.Close();


            string logFile = Path.Combine(Utility.ComponentBaseFolder + "\\Logs", "Crawl_SocialMedia.csv");
            try
            {
                using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(logFile, true))
                {

                    foreach (SocialMediaData s in socialEntries.Values)
                    {
                        if (s.Value != string.Empty && s.DomainID != 0 && s.ShopperID != 0)
                        {
                            file2.WriteLine(s.ToCommaString());
                        }
                    }

                }
            }
            catch (Exception) { }

        }

        [TestMethod]
        public void Facebook_GetWhoWeLike()
        {
            FacebookFanInfo ffi = new FacebookFanInfo(new FacebookTestDataSource());

            string accountId = XMLUtility.GetTextFromAccountNode(ffi.Xml, "id");
            string accessToken = XMLUtility.GetTextFromAccountNode(ffi.Xml, "accesstoken");

            Graph g = new Graph();
            //Page p = g.GetPageLikes(Convert.ToInt64(accountId), accessToken);

        }

        #region private methods

        private string ReadLineFromFile()
        {
            string testLine = string.Empty;
            string line;
            string filePath = Path.Combine(Utility.ComponentBaseFolder + "\\Logs", String.Format("Facebook_TestDataRun_{0:M_d_yyyy}", DateTime.Today) + ".log");

            StreamReader file = new StreamReader(filePath);

            while ((line = file.ReadLine()) != null)
            {
                testLine = line;
            }

            file.Close();
            File.Delete(filePath);

            return testLine;
        }

        private SocialMediaData CleanSocialMediaValue(SocialMediaData socialMediaData)
        {
            if (socialMediaData.Value.ToLower().Contains("facebook") && !socialMediaData.Value.ToLower().Contains("share"))
            {
                if (socialMediaData.Value.Contains(","))
                {
                    socialMediaData.Value = socialMediaData.Value.Split(',')[1];
                }
            }
            else
            {
                socialMediaData.Value = string.Empty;
            }

            if (socialMediaData.Value.Contains("|"))
            {
                socialMediaData.Value = socialMediaData.Value.Split('|')[1];
            }

            return socialMediaData;
        }

        #endregion
    }

    class SocialMediaData
    {
        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _domainName = string.Empty;
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }

        private int _domainID = 0;
        public int DomainID
        {
            get { return _domainID; }
            set { _domainID = value; }
        }

        private int _shopperID = 0;
        public int ShopperID
        {
            get { return _shopperID; }
            set { _shopperID = value; }
        }

        public string ToCommaString()
        {
            string s = string.Format("{0},{1},{2},{3}", Value, DomainID, DomainName, ShopperID);
            return s;
        }
    }
}
