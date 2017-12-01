using Drone.API.Dig.Ssl;
using Drone.API.Dig.WhoIs;
using Drone.API.DNS;
using Drone.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml;

namespace Drone.API.Dig
{
    /// <summary>
    /// Windows Implementation of Linux Dig socket commands for DNS queries
    /// </summary>
    public class Dig : IDisposable
    {
        //internal Resolver _resolver;
        const int DNS_PORT = 53;
        public Dictionary<string, string> Lookups;
        private List<string> _maliciousList;
        internal WhoIsLookup _who;
        private List<IPEndPoint> _dnsServers;

        #region constructor

        public Dig()
        {
            //_resolver = new Resolver();
            _who = new WhoIsLookup();
            LoadXMLLookups();
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            _dnsServers = GetListNameServers();
        }

        #endregion

        /// <summary>
        /// Uses {IP}origin.asn.cymru.com to get ASN and then additional information
        /// from {AS#}asn.cymru.com to get company name
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Company name from AS records</returns>
        public string GetWebHostName(string domainName)
        {
            string companyName = "None";
            string asn = string.Empty;

            try
            {
                IPAddress ip = GetIPAddress(domainName);

                if (!Object.Equals(ip, null))
                {
                    asn = GetASN(ip.ToString());
                    if (asn.Contains(" "))
                    {
                        asn = asn.Split(' ')[0];
                    }

                    DnsQuery _dnsQuery = new DnsQuery(_dnsServers, string.Format("AS{0}.asn.cymru.com", asn));
                    DnsAnswer resp = _dnsQuery.QueryServers(RecordType.TXT);

                    if (!Object.Equals(resp, null) && resp.Answers.Count > 0 && !Object.Equals(resp.Answers[0], null))
                    {
                        TxtRecord txtRecord = resp.Answers[0].Data as TxtRecord;
                        if (!Object.Equals(txtRecord, null))
                            companyName = txtRecord.COMPANY;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetWebHostName", "Domain: " + domainName);
            }

            return GetCompanyFromRecordName(companyName, domainName, DigTypeEnum.WEB);
        }

        /// <summary>
        /// Gets the DNS host name by querying for NS records and crossing a lookup table
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public string GetDNSHostName(string domainName)
        {
            string hostName = "None";
            try
            {
                List<NSRecord> nss = GetNSRecords(domainName);

                if (nss.Count > 0 && !Object.Equals(nss[0], null))
                    hostName = GetCompanyFromRecordName(nss[0].NSDomain, domainName, DigTypeEnum.NS);
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetDNSHostName", "Domain: " + domainName);
            }

            return hostName;
        }

        /// <summary>
        /// Get the Email host name from domain name.  Looks up MX records and then company look up on it
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public string GetEmailHostName(string domainName)
        {
            string hostName = "None";
            try
            {
                List<MXRecord> mxs = GetMXRecord(domainName);

                //todo get MX name from record. tostring??
                if (mxs.Count > 0 && !Object.Equals(mxs[0], null))
                    hostName = GetCompanyFromRecordName(mxs[0].ToString(), domainName, DigTypeEnum.MX);
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetEmailHostName", "Domain: " + domainName);
            }

            return hostName;
        }

        /// <summary>
        /// Gets the IP address for the domain name
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns>IPAddress</returns>
        public IPAddress GetIPAddress(string domainName)
        {
            try
            {
                //try the domain
                ARecord aRecord = GetARecord(domainName);

                //if null try the www subdomain
                if (Object.Equals(aRecord, null))
                    aRecord = GetARecord("www." + domainName);

                if (!Object.Equals(aRecord, null))
                {
                    //compare address to known malicious list before returning
                    if (!_maliciousList.Contains(aRecord.IpAddress.ToString()))
                        return aRecord.IpAddress;
                    else
                        Utility.WriteToLogFile(String.Format("SmallBiz_MaliciousIPs_{0:M_d_yyyy}", DateTime.Today) + ".log"
                                                                    , String.Format("{0} is a known malicious ip associated with {1}", aRecord.IpAddress.ToString(), domainName));
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetIPAddress", "Domain: " + domainName);
            }

            return null;
        }

        /// <summary>
        /// Sends an HTTPRequest to the website on 443 to determine if there is a valid certificate
        /// </summary>
        /// <param name="domainName">any valid domain name without "www."</param>
        /// <returns>SSLCert object with Issuer and Subject (type)</returns>
        public SSLCert GetSSLVerification(string domainName)
        {
            SSLCert myCert = new SSLCert { Issuer = "None", Subject = "None" };
            string groupName = Guid.NewGuid().ToString();
            Uri u = new Uri(string.Format(@"https://www.{0}", domainName));
            ServicePoint sp = ServicePointManager.FindServicePoint(u);

            if (string.IsNullOrEmpty(domainName))
                return myCert;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format(@"https://www.{0}", domainName));

            try
            {
                req.Timeout = 3000;
                req.Method = "HEAD";
                req.ConnectionGroupName = groupName;

                using (WebResponse resp = req.GetResponse()) { }

                myCert = new SSLCert(sp.Certificate, domainName);
            }
            catch (WebException we)
            {
                if (!we.Message.Contains("timed out"))
                {
                    try
                    {
                        req = (HttpWebRequest)HttpWebRequest.Create(string.Format(@"https://{0}", domainName));
                        using (WebResponse resp = req.GetResponse()) { }
                        myCert = new SSLCert(sp.Certificate, domainName);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
            finally
            {
                sp.CloseConnectionGroup(groupName);
            }

            myCert.FixedName = GetCompanyFromRecordName(myCert.IssuerName, domainName, DigTypeEnum.SSL).TrimStart(new char[] { '"', '\\' });

            return myCert;
        }

        public string GetCompanyFromRecordName(string record, string domain, DigTypeEnum recordType)
        {
            if (string.IsNullOrEmpty(record) || record == "None")
                return "None";

            string lookupValue = record;
            string lowerRecord = record.ToLowerInvariant();

            if (lowerRecord.Contains(domain.Split('.')[0].ToLowerInvariant()))
                return "Self Hosted";

            KeyValuePair<string, string> foundItem = new KeyValuePair<string, string>();
            foreach (KeyValuePair<string, string> item in Lookups)
            {
                if (lowerRecord.Contains(item.Key.ToLowerInvariant()))
                {
                    foundItem = item;
                    break;
                }

                if (lowerRecord.Contains(item.Value.ToLowerInvariant()))
                {
                    foundItem = item;
                    break;
                }
            }
            lowerRecord = string.Empty;

            if (!Object.Equals(foundItem.Value, null))
                return foundItem.Value;

            if (recordType == DigTypeEnum.MX || recordType == DigTypeEnum.NS)
            {
                record = record.TrimEnd('.');
                if (record.Count(per => per == '.') > 1)
                {
                    int period = record.IndexOf('.') + 1;
                    return record.Substring(period, record.Length - period).TrimEnd('.');
                }
            }

            if (recordType == DigTypeEnum.WEB)
            {
                //hack to get company name from ASName 
                string[] name = record.Split(new char[] { ' ' }, 2);
                if (name.Count() > 1)
                {
                    lookupValue = name[1];
                    if (lookupValue == "====")
                        lookupValue = name[0];

                    if (lookupValue.StartsWith("- "))
                        return lookupValue.Substring(2);
                }
            }

            return lookupValue;
        }

        public string GetRegistrar(string domainName)
        {
            string registrar = string.Empty;
            if (string.IsNullOrEmpty(domainName))
            {
                registrar = "None";
            }
            else
            {
                try
                {
                    registrar = GetCompanyFromRecordName(_who.GetRegistrar(domainName), domainName, DigTypeEnum.NONE);
                }
                catch (Exception e)
                {
                    ExceptionExtensions.LogError(e, "Dig.GetRegistrar", "Domain: " + domainName);
                }
            }
            return registrar;
        }


        #region internal methods

        internal ARecord GetARecord(string domainName)
        {
            try
            {
                DnsQuery _dnsQuery = new DnsQuery(_dnsServers, domainName);

                if (Object.Equals(null, _dnsQuery)) {
                    ExceptionExtensions.LogError(new ArgumentNullException("dnsQuery is NULL"), "Dig.GetARecord", "dns servers count: " + _dnsServers.Count);
                    return null;
                }

                DnsAnswer answer = _dnsQuery.QueryServers(RecordType.A);

                if (!Object.Equals(answer, null) && answer.Answers.Count > 0)
                {
                    foreach (Answer item in answer.Answers)
                    {
                        if (!Object.Equals(null, item) && item.RecType == RecordType.A && !Object.Equals(null, item.Data))
                        {
                            return item.Data as ARecord;
                        }
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetARecord", "Domain: " + domainName);
            }

            return null;
        }

        /// <summary>
        /// Queries asn.cymru.com for a TXT record
        /// {Reverse-IPaddress}.origin.asn.cymru.com
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        internal string GetASN(string ipAddress)
        {
            string asnResult = string.Empty;
            try
            {
                DnsQuery _dnsQuery = new DnsQuery(_dnsServers, string.Join(".", ipAddress.Split('.').Reverse()) + ".origin.asn.cymru.com");

                if (Object.Equals(null, _dnsQuery))
                {
                    ExceptionExtensions.LogError(new ArgumentNullException("dnsQuery is NULL"), "Dig.GetASN", "dns servers count: " + _dnsServers.Count);
                    return null;
                }

                DnsAnswer resp = _dnsQuery.QueryServers(RecordType.TXT);

                if (!Object.Equals(null, resp) && resp.Answers.Count > 0)
                {
                    TxtRecord txtRecord = resp.Answers[0].Data as TxtRecord;
                    if (!Object.Equals(txtRecord, null))
                        asnResult = txtRecord.ASN;
                }
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetASN", "IPAddress: " + ipAddress);
            }


            return asnResult;
        }

        /// <summary>
        /// Queries for MX records. These records are used in the lookup table for EMAIL HOST
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns>List of RecordMX</returns>
        internal List<MXRecord> GetMXRecord(string domainName)
        {
            List<MXRecord> records = new List<MXRecord>();
            try
            {
                DnsQuery _dnsQuery = new DnsQuery(_dnsServers, domainName);
                DnsAnswer resp = _dnsQuery.QueryServers(RecordType.MX);

                if (!Object.Equals(null, resp) && resp.Answers.Count > 0)
                    records = resp.Answers.Select(rec => rec.Data as MXRecord).ToList();
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetMXRecord()", "Domain: " + domainName);
            }

            return records;
        }

        /// <summary>
        /// Queries for NS records.  These records are used in the lookup table for DNS HOST
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns>List of RecordDNS</returns>
        internal List<NSRecord> GetNSRecords(string domainName)
        {
            List<NSRecord> records = new List<NSRecord>();
            try
            {
                DnsQuery _dnsQuery = new DnsQuery(_dnsServers, domainName);
                DnsAnswer resp = _dnsQuery.QueryServers(RecordType.NS);

                if (!Object.Equals(null, resp) && resp.Answers.Count > 0)
                    records = resp.Answers.Select(rec => rec.Data as NSRecord).ToList();
            }
            catch (Exception e)
            {
                ExceptionExtensions.LogError(e, "Dig.GetNSRecords", "Domain: " + domainName);
            }

            return records;
        }

        #endregion

        #region private methods

        private void LoadXMLLookups()
        {
            Lookups = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();

            //Friendly name lookups
            string sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Dig_name-lookups.xml");

            xmlDoc.Load(sXMLPath);

            XmlNodeList nodeList = xmlDoc.SelectNodes("/lookups/company");

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes["name"].Value != null)
                {
                    XmlNodeList dbaNodes = node.SelectNodes("dba");
                    foreach (XmlNode dbaNode in dbaNodes)
                    {
                        if (dbaNode.Attributes["value"].Value != null)
                            Lookups.Add(dbaNode.Attributes["value"].Value, node.Attributes["name"].Value);
                    }
                }
            }

            //malicious ip list
            _maliciousList = new List<string>();
            sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Dig_malicious.xml");
            xmlDoc.Load(sXMLPath);

            nodeList = xmlDoc.SelectNodes("/malicious/ip");

            foreach (XmlNode node in nodeList)
            {
                if (!String.IsNullOrEmpty(node.InnerText))
                    _maliciousList.Add(node.InnerText);
            }


            //Dig specific values
            sXMLPath = Path.Combine(Utility.ComponentBaseFolder + "\\Xml", "API_Dig.xml");
            xmlDoc.Load(sXMLPath);

            XmlNode urlnode = xmlDoc.SelectSingleNode("/api/registryserviceurl");

            _who.RegServiceURLs.AddRange(urlnode.InnerText.Split('|'));

        }

        public static List<IPEndPoint> GetListNameServers()
        {
            List<IPEndPoint> nameServers = new List<IPEndPoint>();

            IList<IPAddress> machineDnsServers = GetMachineDnsServers();
            foreach (IPAddress ipAddress in machineDnsServers)
                nameServers.Add(new IPEndPoint(ipAddress, DNS_PORT));

            return nameServers;
        }

        public static List<IPAddress> GetMachineDnsServers()
        {
            List<IPAddress> dnsServers = new List<IPAddress>();
            //IPAddressCollection dnsServers = null;

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();

                    foreach (IPAddress ipAddress in properties.DnsAddresses)
                    {
                        dnsServers.Add(ipAddress);
                    }
                }
            }

            return dnsServers;
        }

        #endregion


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Dig()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (_resolver != null)
                //{
                //    _resolver.Dispose();
                //}
            }
        }
    }
}
