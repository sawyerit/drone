using Drone.API.Dig;
using Drone.API.Dig.Ssl;
using Drone.API.DNS;
using Drone.Crunchbase.Components;
using Drone.Shared;
using Drone.Shared.LoggingService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Drone.Test
{
    [TestClass]
    public class DigTests
    {
        Dig _dig;

        [TestInitialize]
        public void InitializeTest()
        {
            _dig = new Dig();
        }

        [TestMethod]
        public void DigConstructor()
        {
            Dig dig = new Dig();

            Assert.IsNotNull(dig.Lookups);
            Assert.IsTrue(dig.Lookups.Count > 0);
        }

        [TestMethod]
        public void DigARecord()
        {
            Dig dig = new Dig();
            ARecord record = dig.GetARecord("facebook-security-team.com"); //threw obj ref on sever
        }

        [TestMethod]
        public void DigASN()
        {
            Dig dig = new Dig();
            string record = dig.GetASN("50.63.202.53");

            Assert.AreEqual("26496", record);
        }

        [TestMethod]
        public void DigWebHostName()
        {
            Dig dig = new Dig();

            string record = dig.GetWebHostName("netflix.com");
        }

        [TestMethod]
        public void DigIPAddress()
        {            
            IPAddress record = _dig.GetIPAddress("takenbakesites.com");

            Assert.IsNotNull(record);
            Assert.IsFalse(String.IsNullOrEmpty(record.ToString()));
            Assert.AreEqual("50.63.202.53", record.ToString());
        }

        [TestMethod]
        public void DigMXRecord()
        {
            Dig dig = new Dig();
            List<MXRecord> records = dig.GetMXRecord("TOMHACK.COM");

            Assert.IsNotNull(records);
            Assert.IsTrue(records.Count > 0);
            Debug.WriteLine(records[0].ToString());
            Assert.IsTrue(records[0].ToString().Contains("secureserver"));
        }

        [TestMethod]
        public void DigNSRecord()
        {
            Dig dig = new Dig();
            List<NSRecord> records = dig.GetNSRecords("takenbakesites.com");

            Assert.IsNotNull(records);
            Assert.IsTrue(records.Count > 0);
        }

        [TestMethod]
        public void DigSSLCert()
        {
            Dig dig = new Dig();

            SSLCert cert = dig.GetSSLVerification("godaddy.com");

            Assert.IsNotNull(cert);
            Assert.IsNotNull(cert.IssuerName);
            Assert.IsNotNull(cert.SubjectType);
            Assert.AreEqual("www.godaddy.com", cert.SubjectType);

            cert = dig.GetSSLVerification("1footout.com");
            Assert.AreEqual(cert.FixedName, "None");

        }

        [TestMethod]
        public void DigWhoIs()
        {
            string registrar = _dig.GetRegistrar("theblissfulbakker.com");
            Assert.IsNotNull(registrar);

            registrar = _dig.GetRegistrar("theblissfulbakker.com");
            Assert.IsTrue(registrar.ToLower().Contains("wild west"));
        }

        [TestMethod]
        public void DigInvalidDomainName_ReturnsNone()
        {
            string result = _dig.GetDNSHostName("abadname");

            Assert.AreEqual("None", result);
        }

        [TestMethod]
        public void FindCompanyInLookups()
        {
            Dig dig = new Dig();

            string foundName = dig.GetRegistrar("1computer.info");
            Assert.AreEqual("Network Solutions", foundName);

            //self hosted
            foundName = dig.GetEmailHostName("fash-art.com");
            Assert.AreEqual("Self Hosted", foundName);

            foundName = dig.GetEmailHostName("blooclick.com");
            Assert.AreEqual("ovh systems", foundName.ToLower());

            //email
            foundName = dig.GetEmailHostName("conduitlabs.com");
            Assert.AreEqual("b-io.co", foundName);

            //not found, use record
            foundName = dig.GetDNSHostName("travellution.com");
            Assert.AreEqual("technorail.com", foundName);

            //found, use company name
            foundName = dig.GetDNSHostName("godaddy.com");
            Assert.AreEqual("Self Hosted", foundName);

            //no SSL issuer
            foundName = dig.GetCompanyFromRecordName(dig.GetSSLVerification("cybergeekshop.com").IssuerName, "cybergeekshop.com", DigTypeEnum.SSL);
            Assert.AreEqual("None", foundName);

            //webhost (split AS name with -)
            foundName = dig.GetWebHostName("cybergeekshop.com");
            Assert.AreEqual("Unified Layer", foundName);

            foundName = dig.GetWebHostName("microteksystems.net");
            Assert.AreEqual("SoftLayer", foundName);

            //webhost (splitting AS Name without -)
            foundName = dig.GetWebHostName("eatads.com");
            Assert.AreEqual("Amazon", foundName);

        }

        [TestMethod]
        [Ignore]
        public void Follow301redirect()
        {
            //kraftymoms.com now shows Internap as host, GoDaddy as dns.  Webs.com uses internap.
            //We can do a head httprequest to see if there's a redirect.  If so, we can get the redirect
            //to page and do a webhost lookup on that. In this case, they both say Internap.
            //even a lookup of webs.com says internap


            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.kraftymoms.com");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.facebook.com"); //feedroom.com
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();

            string result = BuildTextFromResponse(resp);


            int code = (int)resp.StatusCode;
            string host = resp.Headers["Location"];

        }

        [TestMethod]
        public void DigWebHostName_Other()
        {
            //resolver will cache these, so remove the cache lookup
            //dig does a friendly name lookup as well, that can be removed for a VERY slight speed increase over a large #
            Dig dig = new Dig();

            List<string> domainList = new List<string>();
            List<string> webhostList = new List<string>();
            List<string> dnshostList = new List<string>();

            //domainList.Add("mprconsultinghk.com");
            domainList.Add("weebly.com");
            domainList.Add("kraftymoms.com");
            domainList.Add("paranique.com");
            domainList.Add("casaartandphoto.com");


            foreach (var item in domainList)
            {
                webhostList.Add(dig.GetWebHostName(item));
            }

            foreach (var item in domainList)
            {
                dnshostList.Add(dig.GetDNSHostName(item));
            }



        }

        [TestMethod]
        public void IsIPParked()
        {
            Dig dig = new Dig();
            IPAddress record = dig.GetIPAddress("takenbakesites.com");

            bool isParked = IPAddressRange.IsInRange(record);
            IPAddressRange.IsInRange(record);

            Assert.IsTrue(isParked);

            record = dig.GetIPAddress("godaddy.com");

            isParked = IPAddressRange.IsInRange(record);
            IPAddressRange.IsInRange(record);

            Assert.IsTrue(!isParked);

        }

        [TestMethod]
        [Ignore]
        public void DigURL_PerformanceTest()
        {
            //resolver will cache these, so remove the cache lookup
            //dig does a friendly name lookup as well, that can be removed for a VERY slight speed increase over a large #
            Dig dig = new Dig();
            Crunch crunch = new Crunch();

            List<string> domainList = new List<string>();
            Dictionary<string, string> webhostList = new Dictionary<string, string>();
            List<string> errorList = new List<string>();

            domainList.Add("mprconsultinghk.com");
            domainList.Add("tnipresents.com");
            domainList.Add("kraftymoms.com");
            domainList.Add("paranique.com");
            domainList.Add("eatads.com");
            domainList.Add("travellution.com");
            domainList.Add("bee.com");
            domainList.Add("yahoo.com");
            domainList.Add("google.com");
            domainList.Add("microsoft.com");

            DateTime endTime;
            DateTime startTime = DateTime.Now;
            string header = string.Empty;
            int statCode = 0;

            startTime = DateTime.Now;
            Parallel.For(0, 100, (i) =>
            {
                foreach (var item in domainList)
                {
                    //try
                    //{
                    //    CheckHead(ref statCode, ref header, item);
                    //}
                    //catch (Exception e)
                    //{
                    //    if (e.Message.Contains("timed out"))
                    //    {
                    //        try
                    //        {
                    //            CheckHead(ref statCode, ref header, item);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            if (ex.Message.Contains("timed out"))
                    //            {
                    //                errorList.Add("headrequest timed out");
                    //            }
                    //        }

                    //    }
                    //    else
                    //    {
                    //        errorList.Add(e.Message);
                    //    }
                    //}

                    try
                    {
                        //add host to list
                        //if (statCode == 301)
                        //    webhostList.Add(dig.GetWebHostName(Utility.CleanUrl(header)), item);
                        //else
                        webhostList[item + i.ToString()] = dig.GetWebHostName(item);
                    }
                    catch (Exception)
                    {
                        webhostList[item] = "webhost timed out";
                    }
                }
            });
            endTime = DateTime.Now;
            TimeSpan elapsedTime1 = endTime.Subtract(startTime);

            //List<KeyValuePair<string, string>> timeoutList = webhostList.Where(item => item.Key == "headrequest timed out").ToList();
            List<KeyValuePair<string, string>> webtimeoutList = webhostList.Where(item => item.Value == "webhost timed out").ToList();
        }

        [TestMethod]
        [Ignore]
        public void DigWRegistrar_PerformanceTest()
        {
            //resolver will cache these, so remove the cache lookup
            Dig dig = new Dig();
            Crunch crunch = new Crunch();

            List<string> domainList = new List<string>();
            List<string> webhostList = new List<string>();

            domainList.Add("coderow.com");
            domainList.Add("slashcommunity.com");
            domainList.Add("vantronix.com");
            domainList.Add("sociofy.com");
            domainList.Add("netconstructor.com");
            domainList.Add("dotfox.com");
            domainList.Add("go.co");
            domainList.Add("1computer.info");
            domainList.Add("andyet.net");
            domainList.Add("p1us.me");
            domainList.Add("10cms.com");
            domainList.Add("1010data.com");
            domainList.Add("1800vending.com");
            domainList.Add("easybacklog.com");
            domainList.Add("abcotechnology.com");
            domainList.Add("abcsignup.com");
            domainList.Add("airtag.com");
            domainList.Add("nuospace.com");
            domainList.Add("brightscope.com");
            domainList.Add("data180.com");
            domainList.Add("chicagolandlordsattorney.com");

            DateTime endTime;
            DateTime startTime = DateTime.Now;
            Random rand = new Random();

            foreach (var item in domainList)
            {
                webhostList.Add(dig.GetDNSHostName(item));
            }

            endTime = DateTime.Now;
            TimeSpan elapsedTime1 = endTime.Subtract(startTime);
        }

        private static void CheckHead(ref int statCode, ref string header, string item)
        {
            //check for redirect
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://" + item);
            request.AllowAutoRedirect = false;
            request.Method = "HEAD";
            request.Timeout = 5000;

            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                header = resp.Headers["Location"];
                statCode = (int)resp.StatusCode;
            }
        }

        [TestMethod]
        [Ignore]
        public void Dig_ChangeHostEntryTest()
        {
            var logClient = new BILoggerServiceClient();
            logClient.HandleException("testing", "none", "Drone Processor", LogTypeEnum.Information, LogActionEnum.Email, "DigTest", "Testing Endpoint", "", "ssawyer", "");

            logClient = new BILoggerServiceClient();
            var wsb = logClient.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;

            wsb.ProxyAddress = new Uri("http://172.19.67.10");
            wsb.BypassProxyOnLocal = false;
            wsb.UseDefaultWebProxy = false;

            //System.Net.WebProxy webProxy = new System.Net.WebProxy("http://172.19.67.10", true);
            ////webProxy.Credentials = new System.Net.NetworkCredential("1", "1");
            //System.Net.WebRequest.DefaultWebProxy = webProxy;

            //just to double check IP from the host file
            IPHostEntry ip2 = Dns.GetHostEntry("bizintel-test.intranet.gdg");

            logClient.HandleException("testing", "none", "Drone Processor", LogTypeEnum.Information, LogActionEnum.Email, "DigTest", "Testing Endpoint", "", "ssawyer", "");

        }

        [TestMethod]
        [Ignore]
        public void IPLookup_FromZoneSample()
        {
            Dig dig = new Dig();
            //copy the file because we are going to delete as we go instead of holding a million records in memory
            string masterFile = "PortSample.txt";
            string tempFile = "temp_" + masterFile;
            string ipFile = "tempIP_" + masterFile;
            string tempLine = string.Empty;


            List<string> webhostList = new List<string>();
            List<string> errorList = new List<string>();
            string header = string.Empty;
            int counter = 0;

            // Ensure that the target does not exist.
            File.Copy(masterFile, tempFile, true);
            try
            {
                //read lines from tempfile & parse it					
                using (StreamReader r = new StreamReader(ipFile))
                {
                    // Use while != null pattern for loop
                    while ((tempLine = r.ReadLine()) != null)
                    {
                        if (!String.IsNullOrWhiteSpace(tempLine))
                        {
                            //we have a line, parse the tabs and call webhost lookups
                            //string[] seperator = { "\t" };
                            string[] seperator = { "|" };
                            string[] cols = tempLine.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                            counter++;

                            if (cols.Length > 0)
                            {
                                Utility.WriteToLogFile("ipOnly_" + ipFile, cols[0]);
                            }
                        }
                    }

                    List<string> timeoutList = webhostList.Where(item => item == "headrequest timed out").ToList();
                    List<string> webtimeoutList = webhostList.Where(item => item == "webhost timed out").ToList();

                }

                File.Delete(tempFile);

            }
            catch (Exception)
            {

                throw;
            }
            //do lookup

            //write data


        }

        [TestMethod]
        [Ignore]
        public void WebHostLookup_FromZoneSample()
        {
            Dig dig = new Dig();
            //copy the file because we are going to delete as we go instead of holding a million records in memory
            string masterFile = "PortSample.txt";
            string tempFile = "temp_" + masterFile;
            string tempLine = string.Empty;


            List<string> webhostList = new List<string>();
            List<string> errorList = new List<string>();
            string header = string.Empty;
            int statCode = 0;
            int counter = 0;

            // Ensure that the target does not exist.
            File.Copy(masterFile, tempFile, true);
            try
            {
                //read lines from tempfile & parse it					
                using (StreamReader r = new StreamReader(tempFile))
                {
                    // Use while != null pattern for loop
                    while ((tempLine = r.ReadLine()) != null)
                    {
                        if (!String.IsNullOrWhiteSpace(tempLine))
                        {
                            //we have a line, parse the tabs and call webhost lookups
                            //string[] seperator = { "\t" };
                            string[] seperator = { "|" };
                            string[] cols = tempLine.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                            counter++;

                            if (cols.Length > 0)
                            {
                                IPAddress addr = null;
                                addr = dig.GetIPAddress(Utility.CleanUrl(cols[0]));
                                if (addr != null)
                                    header = string.Empty;
                                statCode = 0;
                                try
                                {
                                    CheckHead(ref statCode, ref header, cols[0]);
                                }
                                catch (Exception)
                                {
                                    errorList.Add("headrequest timed out");
                                }

                                try
                                {
                                    //add host to list
                                    if (statCode == 301)
                                        webhostList.Add(dig.GetWebHostName(Utility.CleanUrl(header)));
                                    else
                                        webhostList.Add(dig.GetWebHostName(cols[0]));
                                }
                                catch (Exception)
                                {
                                    webhostList.Add("webhost timed out");
                                }
                            }
                        }
                    }

                    List<string> timeoutList = webhostList.Where(item => item == "headrequest timed out").ToList();
                    List<string> webtimeoutList = webhostList.Where(item => item == "webhost timed out").ToList();

                }

                File.Delete(tempFile);

            }
            catch (Exception)
            {

                throw;
            }
            //do lookup

            //write data


        }

        [TestMethod]
        [Ignore]
        public void CookieReader_test()
        {
            //make sure its not a blocked page
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.spacecoastpetes.com/"); //zen cart (zenid)
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.focusattack.com/"); //bigcommerce (shop_session_token)
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.whatajewel.com"); //osCommerce (osCsid) also has perm 301redirect
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.givethisgift.co.uk/"); //osCommerce (osCsid) no redirect
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.millardauctionco.com"); //webs.com (fwww)



            CookieContainer cookieJar = new CookieContainer();

            request.UserAgent = "User-Agent	Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            request.CookieContainer = cookieJar;
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();


            CookieCollection cc1 = cookieJar.GetCookies(request.RequestUri);
            CookieCollection cc2 = cookieJar.GetCookies(resp.ResponseUri);



            string result = BuildTextFromResponse(resp);

        }

        private string BuildTextFromResponse(HttpWebResponse response)
        {
            string responseText = string.Empty;

            using (var streamReader = new StreamReader(response.GetResponseStream()))
                responseText = streamReader.ReadToEnd();

            return responseText;
        }
    }


    public static class IPAddressRange
    {
        static List<IPRange> parkedRanges = new List<IPRange>();

        public static bool IsInRange(IPAddress address)
        {
            if (parkedRanges.Count == 0)
            {
                parkedRanges.Add(new IPRange
                {
                    AddressFamily = IPAddress.Parse("50.63.202.1").AddressFamily,
                    AddressLower = IPAddress.Parse("50.63.202.1"),
                    AddressUpper = IPAddress.Parse("50.63.202.127")
                });

                parkedRanges.Add(new IPRange
                {
                    AddressFamily = IPAddress.Parse("184.168.221.1").AddressFamily,
                    AddressLower = IPAddress.Parse("184.168.221.1"),
                    AddressUpper = IPAddress.Parse("184.168.221.127")
                });
            }

            //byte[] addressBytes = address.GetAddressBytes();
            long ip = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

            foreach (IPRange range in parkedRanges)
            {
                //bool cont = false;
                if (address.AddressFamily != range.AddressFamily)
                    continue;

                long ipStart = BitConverter.ToInt32(range.AddressLower.GetAddressBytes().Reverse().ToArray(), 0);
                long ipEnd = BitConverter.ToInt32(range.AddressUpper.GetAddressBytes().Reverse().ToArray(), 0);

                if (ip >= ipStart && ip <= ipEnd)
                    return true;
            }

            return false;
        }
    }

    public class IPRange
    {
        public IPAddress AddressLower { get; set; }
        public IPAddress AddressUpper { get; set; }
        public AddressFamily AddressFamily { get; set; }
    }


}
