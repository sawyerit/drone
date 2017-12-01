using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Drone.Data;
using Drone.Entities.Twitter.v11;
using Drone.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Drone.Test
{
    [TestClass]
    public class MiscTests
    {

        [TestMethod]
        public void CalculateStep_test()
        {
            double stepSize;
            stepSize = CalculateStep(600, 1700, 5);
            stepSize = CalculateStep(600, 1700, 4);

            stepSize = CalculateStep(650, 900, 5);
            stepSize = CalculateStep(650, 900, 4);
        }

        private double CalculateStep(double min, double max, double targetSteps)
        {
            double step = 0;
            double range = Math.Abs(Math.Ceiling(max) - Math.Floor(min));
            if (range != 0)
            {
                double tempStep = range / targetSteps;

                // get the magnitude of the step size
                double mag = (double)Math.Floor(Math.Log10(tempStep));
                double magPow = (double)Math.Pow(10, mag);

                // calculate most significant digit of the new step size
                double magMsd = (int)(tempStep / magPow + 0.5D);

                // promote the MSD to either 1, 2, or 5
                if (magMsd > 5D) magMsd = 10D;
                else if (magMsd > 2D) magMsd = 5D;
                else if (magMsd > 1D) magMsd = 2D;

                // calculate step size
                step = magMsd * magPow;
            }
            return step;
        }

        [TestMethod]
        public void CalculateDate_LeapYear()
        {

        }

        [TestMethod]
        public void BitWiseTest()
        {
            Assert.AreEqual(2, 14 & 2);
            Assert.AreEqual(14, 12 | 2);
            Assert.AreEqual(14, 14 | 2);

            Assert.AreEqual(12, 14 & 12);
            Assert.AreEqual(14, 14 | 12);
        }

        [TestMethod]
        public void SortTest()
        {


            List<SortObject> srtList = new List<SortObject>();
            srtList.Add(new SortObject { id = 1, Name = "Shane" });
            srtList.Add(new SortObject { id = 12, Name = "Brian" });
            srtList.Add(new SortObject { id = 2, Name = "Brian" });
            srtList.Add(new SortObject { id = 9, Name = "Mike" });
            srtList.Add(new SortObject { id = 5, Name = "Mike" });
            srtList.Add(new SortObject { id = 3, Name = "Adam" });
            srtList.Add(new SortObject { id = 4, Name = "Adam" });

            Assert.AreEqual(srtList[0].Name, "Shane");

            srtList.Sort();

            Assert.AreEqual(srtList[0].Name, "Adam");
        }

        [TestMethod]
        public void ServiceKill()
        {
            try
            {
                ServiceController service = new ServiceController("Drone.QueueProcessor.Service", "p3pwsvcweb004");
                if (!Object.Equals(null, service))
                {
                    if (service.Status != ServiceControllerStatus.Stopped && service.Status != ServiceControllerStatus.StopPending)
                    {
                        service.Stop();
                    }
                    else if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                    {
                        service.Start();
                    }
                }
            }
            catch (Exception e)
            {
                // ...
            }
        }

        [TestMethod]
        public void AnonObject()
        {
            var v = new { house = 1234, street = "test street" };
            Assert.AreEqual("test street", v.street);
        }

        [TestMethod]
        public void WhoIsServersTest()
        {
            var host = Whois("0731CARD.COM");
        }

        [TestMethod]
        public void SerializetoXml()
        {
            string obj = "";

            Utility.SerializeToXMLString<KeywordStatus>(obj);
        }

        [TestMethod]
        public void CleanTwitterTag()
        {
            string cleanTag = "https://www.twitter.com/#!/@testuser";
            ValidateAndCleanTwitter(ref cleanTag);
            Assert.AreEqual(cleanTag, "testuser");

            cleanTag = "http://twitter.com/testuser";
            ValidateAndCleanTwitter(ref cleanTag);
            Assert.AreEqual(cleanTag, "testuser");

            cleanTag = "http///www/twitter.com/testuser";
            ValidateAndCleanTwitter(ref cleanTag);
            Assert.AreEqual(cleanTag, "testuser");

            cleanTag = "http:/www.twitter.com/testuser";
            ValidateAndCleanTwitter(ref cleanTag);
            Assert.AreEqual(cleanTag, "testuser");
        }

        [TestMethod]
        public void LogErrors()
        {
            ExceptionExtensions.LogError(new ArgumentException("missing arg"), "MiscTests.LogErrors", "AdditionalInfo from error");
            ExceptionExtensions.LogInformation("MiscTests.LogErrors", "LoggingInformation");
            ExceptionExtensions.LogWarning(new ArgumentNullException("arg null"), "MiscTests.LogErrors", "Warning message additional info");
            ExceptionExtensions.LogError(new System.TimeoutException("timeout"), "MiscTests.LogErrors");
        }

        [TestMethod]
        public void NLP_Processing()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            string html = GetPage("http://www.hotels.com");

            sw.Start();
            for (int i = 0; i < 3000; i++)
            {
                ProcessTopWords(html);
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;

        }

        [TestMethod]
        public void DateToString_Test()
        {
            DateTime dt = DateTime.MinValue;
            DateTime.TryParse(string.Format("{0}-{1}-{2}", "2011", "02", "14"), out dt);

            string formattedDate = DateTime.Now.ToString("MMM yyyy");
        }

        [TestMethod]
        public void TaskCancellation_Test()
        {
            //Start 100 task thredss and call DoWork on each.
            //on task end, continue and check the result. If we have a found result, call cancel
            //DoWork checks cancel at opportune times and will exit out of the loop. short circuiting the rest of the threads.
            Stopwatch s = new Stopwatch();
            string taskTimes = string.Empty;
            string parallelTimes = string.Empty;
            for (int q = 0; q < 10; q++)
            {
                CancellationTokenSource _builderCancellation = new CancellationTokenSource();
                Task<string>[] tasks = new Task<string>[100];
                string result = string.Empty;

                s.Restart();
                for (int i = 0; i < 100; i++)
                {
                    int j = i;
                    tasks[j] = Task.Factory.StartNew<string>(() =>
                    {
                        if (DoWork(j, _builderCancellation.Token))
                        {
                            return "worked: " + j;
                        }
                        return string.Empty;
                    }, _builderCancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                        .ContinueWith<string>(task =>
                        {
                            if (!string.IsNullOrEmpty(task.Result))
                            {
                                result = task.Result;
                                _builderCancellation.Cancel(true);
                            }
                            //return result;
                            return task.Result;
                        });
                }
                Task.WaitAll(tasks);

                s.Stop();
                taskTimes += " " + s.Elapsed.ToString();
            }

            //task call
            for (int q = 0; q < 10; q++)
            {
                string result = string.Empty;

                s.Restart();
                Parallel.For(0, 100, (z, loopState) =>
                {
                    if (DoWork(z, loopState))
                    {
                        result = "worked: " + z;
                        loopState.Stop();
                    }
                });

                s.Stop();
                parallelTimes += " " + s.Elapsed.ToString();
            }
        }

        [TestMethod]
        public void StringParse_Test()
        {
            string URL = "http://www.testdomainname/folder/page.asp";
            int indx = URL.IndexOf("//");
            URL = URL.Remove(0, indx + 2);

            Assert.AreEqual(URL, "www.testdomainname/folder/page.asp");
        }

        [TestMethod]
        public void CleanFiles_Test()
        {
            Utility.lockList.Add("test1", new LockItem { Name = "test1", TimeStamp = DateTime.Now });
            Utility.lockList.Add("test2", new LockItem { Name = "test2", TimeStamp = DateTime.Now.AddHours(-1) });
            Utility.lockList.Add("test3", new LockItem { Name = "test3", TimeStamp = DateTime.Now.AddDays(-2).AddHours(-1) });
            Utility.lockList.Add("test4", new LockItem { Name = "test4", TimeStamp = DateTime.Now.AddDays(-3) });

            Utility.RemoveOldLogs();
        }

        [TestMethod]
        public void RegexRemoveInvalidChars()
        {
            string value = "facebook|http://www.facebook.com/CigarBeads"; //hidden hex 16 chars in front of the http
            value = Regex.Replace(value, "[^\x20-\x7E]", String.Empty);

            Assert.AreNotEqual("facebook|http://www.facebook.com/CigarBeads", value);
        }

        private void ThreadedFileAccessor(object obj)
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(@"\\p3pwsvcweb001\d$\bizintel.webservices.gdg\Drone\QueueProcessor\Logs");
                Assert.IsTrue(files.Length > 0);
            }
            catch (Exception e)
            {
                throw e;
            }            
        }

        [TestMethod]
        [Ignore]
        public void GetAllPortfolioByType()
        {
            WebAPI.Services.PortfolioService ps = new WebAPI.Services.PortfolioService();
            ps.GetAllPortfolioByType("14");
        }

        [TestMethod]
        public void ParseDataFromXML()
        {
            //read the xml
            XmlDocument xmlDoc = new XmlDocument();
            string sXMLPath = Path.Combine(@"C:\Projects\Notes and Scripts\Projects\Locu", "toptable_20131107.xml");

            using (XmlTextReader tr = new XmlTextReader(sXMLPath))
            {
                tr.Namespaces = false;
                xmlDoc.Load(tr);
            }

            //find all restaurants
            XmlNodeList stores = xmlDoc.SelectNodes("//RestaurantFeed/Restaurant");

            Dictionary<string, string> restDomains = new Dictionary<string, string>();
            Dictionary<string, string> restDomainsCopy = new Dictionary<string, string>();

            //first load list of GDDomains
            var reader = new StreamReader(File.OpenRead(Path.Combine(@"C:\Projects\Notes and Scripts\Projects\Locu", "gddomains.csv")));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values[2].ToLower().Contains("top"))
                {
                    restDomains.Add(values[1].ToLower(), string.Empty);
                }
            }

            //write to file
            foreach (XmlNode node in stores)
            {
                XmlNode urlNode = node.SelectSingleNode("URL");
                XmlNode ridNode = node.SelectSingleNode("RestaurantID");

                if (!Object.Equals(null, urlNode) && !Object.Equals(null, ridNode))
                {
                    string cleanedUrl = Utility.CleanUrl(urlNode.InnerText.ToLower()).Trim();
                    if (restDomains.ContainsKey(cleanedUrl))
                    {
                        restDomains[cleanedUrl] = ridNode.InnerText;
                    }
                }
            }

            foreach (var item in restDomains)
            {
                restDomainsCopy.Add(item.Key, item.Value);
            }

            foreach (var item in restDomains)
            {
                if (String.IsNullOrWhiteSpace(item.Value))
                {
                    foreach (XmlNode node in stores)
                    {
                        XmlNode urlNode = node.SelectSingleNode("URL");
                        XmlNode ridNode = node.SelectSingleNode("RestaurantID");
                        if (!Object.Equals(null, urlNode) && !Object.Equals(null, ridNode))
                        {
                            string cleanedText = Utility.CleanUrl(urlNode.InnerText.ToLower()).Trim();
                            if (!String.IsNullOrWhiteSpace(cleanedText) && !restDomains.ContainsKey(ridNode.InnerText))
                            {
                                if (cleanedText.Contains(item.Key))
                                {
                                    restDomainsCopy[item.Key] = ridNode.InnerText;
                                    break;
                                }
                            }
                        }
                    }
                }
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Projects\Notes and Scripts\Projects\Locu\toptable_20131107_URLRID.csv", false))
            {
                foreach (KeyValuePair<string, string> rest in restDomainsCopy)
                {
                    file.WriteLine(rest.Value + "," + rest.Key + ",TopTable");
                }
            }

        }

        //DoWork for the Task Cancellation test
        //check the token while doing work to see if a cancel has been requested.
        //if so... get out
        private bool DoWork(int i, CancellationToken ct)
        {
            for (int j = 0; j < i; j++)
            {
                if (ct.IsCancellationRequested) return false;
                Thread.Sleep(1000);
            }

            //Debug.WriteLine(i);
            if (i == 3)
                return true;
            else
                return false;
        }

        private bool DoWork(int i, ParallelLoopState state)
        {
            for (int j = 0; j < i; j++)
            {
                if (state.IsStopped) return false;
                Thread.Sleep(1000);
            }

            //Debug.WriteLine(i);
            if (i == 3)
                return true;
            else
                return false;
        }

        private static string Whois(string domain)
        {
            if (domain == null)
                throw new ArgumentNullException();
            int ccStart = domain.LastIndexOf(".");
            if (ccStart < 0 || ccStart == domain.Length)
                throw new ArgumentException();
            string ret = "";
            Socket s = null;
            try
            {
                string cc = domain.Substring(ccStart + 1);
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(new IPEndPoint(Dns.Resolve(cc + ".whois-servers.net").AddressList[0], 43));
                s.Send(Encoding.ASCII.GetBytes(domain + "\r\n"));
                byte[] buffer = new byte[1024];
                int recv = s.Receive(buffer);
                while (recv > 0)
                {
                    ret += Encoding.ASCII.GetString(buffer, 0, recv);
                    recv = s.Receive(buffer);
                }
                s.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                throw new SocketException();
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            return ret;
        }

        private bool ValidateAndCleanTwitter(ref string href)
        {
            StringBuilder sb = new StringBuilder(href);

            sb.Replace("https", string.Empty);
            sb.Replace("http", string.Empty);
            sb.Replace("www", string.Empty);
            sb.Replace("twitter.com", string.Empty);
            sb.Replace("#", string.Empty);
            sb.Replace("!", string.Empty);
            sb.Replace("@", string.Empty);
            sb.Replace("/", string.Empty);
            sb.Replace(".", string.Empty);
            sb.Replace(":", string.Empty);

            href = sb.ToString();

            return true;
        }

        private string GetPage(string fullUrl)
        {
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
            request.AllowAutoRedirect = true;
            request.Method = "GET";
            request.Timeout = 20000;
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.94 Safari/537.4";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ContentType.Contains("text/html"))
                {
                    responseText = BuildTextFromResponse(response);
                }
            }

            return responseText;
        }

        private string BuildTextFromResponse(HttpWebResponse response)
        {
            string responseText = string.Empty;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream urlStream = response.GetResponseStream())
                {
                    int read; int totalRead = 0;
                    while (true)
                    {
                        read = urlStream.Read(buffer, 0, buffer.Length);
                        totalRead += read;

                        if (read <= 0)
                        {
                            responseText = Encoding.UTF8.GetString(ms.ToArray());
                            break;
                        }
                        else
                            ms.Write(buffer, 0, read);
                    }
                }
            }

            return responseText;
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //performance test 2
        private List<KeyValuePair<string, int>> ProcessTopWords(string html)
        {
            List<string> stopwords = "a,able,about,across,after,all,almost,also,am,among,an,and,any,are,as,at,be,because,been,but,by,can,cannot,could,dear,did,do,does,either,else,ever,every,for,from,get,got,had,has,have,he,her,hers,him,his,how,however,i,if,in,into,is,it,its,just,least,let,like,likely,may,me,might,most,must,my,neither,no,nor,not,of,off,often,on,only,or,other,our,own,rather,said,say,says,she,should,since,so,some,than,that,the,their,them,then,there,these,they,this,tis,to,too,twas,us,wants,was,we,were,what,when,where,which,while,who,whom,why,will,with,would,yet,you,your".Split(',').ToList();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            doc.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#comment")
                .ToList()
                .ForEach(n => n.Remove());

            string txt = doc.DocumentNode.InnerText;
            txt = txt.Replace("\n", " ");
            txt = txt.Replace("\r", " ");
            txt = txt.Replace("\t", " ");
            txt = txt.Replace(".", " ");
            txt = txt.Replace("<form>", " ");
            txt = txt.Replace("</form>", " ");

            txt = HttpUtility.HtmlDecode(txt);
            txt = txt.ToLowerInvariant();

            char[] arr = txt.ToCharArray();

            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))));
            txt = new string(arr);

            List<string> words = txt.Split(' ').ToList();
            //words.RemoveAll(n => String.IsNullOrWhiteSpace(n) || n.Length < 3);        

            var dict = new Dictionary<string, int>();

            foreach (var word in words)
                if (dict.ContainsKey(word))
                    dict[word]++;
                else if (!String.IsNullOrWhiteSpace(word) && word.Length >= 3 && !IsDigitsOnly(word) && !stopwords.Contains(word))
                    dict[word] = 1;

            List<KeyValuePair<string, int>> myList = dict.ToList();
            myList.Sort((x, y) => y.Value.CompareTo(x.Value));

            return myList;
        }
    }

    public class SortObject : IComparable<SortObject>
    {
        public string Name { get; set; }
        public int id { get; set; }

        public int CompareTo(SortObject other)
        {
            if (this.Name == other.Name)
            {
                return this.id.CompareTo(other.id);
            }
            return this.Name.CompareTo(other.Name);
        }
    }
}
