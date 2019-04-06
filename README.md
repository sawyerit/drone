Drone - BigData Crawler !
=====

###How does DRONE work?

There are two parts to DRONE, the web crawler (aka CrawlDaddy) and the social media branch. 

####Social Media:

DRONE Social Media calls 3rd party API's to gather specific information about [company] on a configurable schedule.

####Web Crawler:

DRONE Crawler crawls a domain portfolio processing the hosted site pages for key information, which it then stores as a key value pair in a SQL database. How a page/site is "processed" is completely up to the implementation of each processor. Some scan the 
page for keywords, some scan meta tags, and others check for site cookies. Since DRONE is completely extensible a developer could implement a processor to "process" the pages for any data that could be parsed from a website. DRONE completes a full portfolio scan in approximately one months time. Since the portfolio contains more than 55 million domains there is no historical data being archived at this time.

_Technically_

DRONE was developed using a Producer-Consumer pattern and utilizes the Unity Application Block as a dependency injection mechanism to facilitate extensibility. It is composed of a DomainProducer, a DomainConsumer, a PersistenceProvider and a variable number of Processors.

_The interfaces_

IDomainProvider - Implementer of this interface provides domains for the crawlers
IBulkCrawler - Implementer of this interface is the Consumer of the domains.
ICrawlProcessor - Each class that interacts with a crawled domain/page implements this interface
IPersistenceProvider - Each crawl processor calls a configured persistence provider for saving the data it "processes"

Abot WebCrawler - Open source C# web crawler built for speed and flexibility. written by Steven Jones. https://code.google.com/p/abot/

_The inner working basics_

An implementation of the producer pulls domains from a source (csv file, database, etc.) and queues a list of domains. Multiple threads of consumers then pull from this queue and feed the domains to their instance of the abot web crawler. Once the web crawler has completed crawling a page and/or domain it notifies any listening processors. The processors will "process" the page for information they care about and then save the information by calling the configured persistence provider.

####What kind of data does DRONE collect?

DRONE Social Media authenticates with and calls Twitter, Facebook, YouTube and Crunchbase API's and records this data in SQL tables.

DRONE Crawler collects DNS data as well as any data that can be identified by keywords, matching signatures, meta tags and cookies when crawling a website. All data is only as current as the last crawl date of the domain. 

####What kind of information can I get from DRONE data?

The bi data discovery page details a large portion of the API calls either completed, in progress, or planned for accessing and contributing to DRONE data.
- http://bizintel-ws.intranet.gdg/bidata/home/discovery

DRONE Social Media gathers tweets about [company] Facebook page likes and demographics, YouTube video views and likes, as well as Crunchbase small biz statistics.

DRONE Crawler source domain data includes an Inactive/Active status, Domain Name, Domain ID, Shopper ID, and Private Label ID and is refreshed daily from the Domains databases.

The data below is collected by Processors that have been developed and plugged in to the DRONE framework:

* IP Address - IPAddress from a dns dig lookup
* DNS Host - the DNS records found for the domain
* WebHost - The web host is determined by looking up the Autonomous System Name (ASN) for an IP. This ASN mapping is done by Cymru
* Email Host - Obtained from the MX records. Note that this isn't extremely useful as CPanel email users will be represented as "self-hosting" their domains.
* Shopping Cart products - Shopping cart products are determine by scraping the served pages for keywords, links, cookies and meta tags that identify the site as using a particular product. (These are defined in an XML rules file located within the project) 

_Current shopping carts being identified are:_

* Shopify
* Volusion 
* Bigcommerce
* WP Ecommerce (WordPress)
* Magento
* ZenCart

_SiteBuilder products_ - Site builder products are determine by scraping the served pages for keywords, links, cookies, meta tags and DNS records. Current site builders being identified are:

* Wix
* Weebly
* webs.com
* Yola
* 1and1
* jimdo
* WordPress
* Joomla
* Drupal

Social Media links - This processor looks for Twitter, Facebook, LinkedIn, YouTube, Flickr tags on a website that might refer to that businesses social media presence.
SSL info - An SSL https request is performed on each domain and the returned information is parsed.
Vertical Info - Meta tags, keywords, and page text is scanned for keywords that match a defined list of vertical information (original vertical words were provided by SEV).
Constant Contact - The page is scanned for keywords matching the constant contact signature.

####Querying DRONE data

_DRONE Crawler data_

Server.Database: 
Source domain table: rptGdDomains
Found data table: rptGdDomainAttributes
Lookup table: rptMarketShareType

SELECT * FROM [gg].[dbo].[rptMarketShareType] 

3 - WebHost 4 - EmailHost 5 - DNSHost 7 - SSLIssuer

8 - CertificateType 12 - SiteBuilder 13 - Ecommerce 14 - Verticals

15 - Social 19 - MailProvider 20 - ParkedType 21 - IPAddress 

 

Source domains are updated daily from the Domains databases, including the Active/Inactive status. Most queries on the DRONE data should include checking the IsActive field.


/*

all shoppers (with specific shopping cart) 

*/

SELECT DISTINCT(shopperid)
FROM rptGDDomainAttributes a with(nolock) 
JOIN rptGDDomains b ON a.rptGdDomainsID = b.ID
WHERE a.TypeID = 13 AND b.IsActive = 1 AND Value = 'magento'
GROUP BY shopperid

/* 

all parked pages 

*/

 SELECT a.Value, b.DomainName, b.ShopperID, b.LastCrawlDate, b.isActive
FROM rptGDDomainAttributes a with (nolock) 
JOIN rptGDDomains b 
ON a.rptGdDomainsID = b.ID
WHERE a.TypeID = 20
ORDER BY b.LastCrawlDate desc

/*

List of WebHosts with count of occurrences (domains) hosted with them

*/

 SELECT a.Value, count(a.Value) as cnt
FROM rptGDDomainAttributes a with (nolock)
JOIN rptGDDomains b ON a.rptGdDomainsID = b.ID 
WHERE a.TypeID = 3 
GROUP by a.Value
HAVING COUNT(a.Value) > 10

/*

verticals matching "cleaning, seattle" 

*/

SELECT a.Value, b.DomainName, b.isActive, b.ShopperID
FROM rptGDDomainAttributes a with (nolock) 
JOIN rptGDDomains b ON a.rptGdDomainsID = b.ID
WHERE a.TypeID = 14 and b.isActive = 1
and a.Value like '%localCity|seattle%'
and a.Value like '%cleaning%'
and a.rptGdDomainsID in (SELECT DISTINCT(rptGdDomainsID) FROM rptGDDomainAttributes WHERE TypeID = 3 AND
Value = 'XXX')

 

/*

top 1000 using wix

*/

 SELECT top(1000) b.ShopperID, b.DomainID, b.DomainName, a.VALUE, b.LastCrawlDate
FROM rptGDDomainAttributes a with(nolock)
JOIN rptGDDomains b ON a.rptGdDomainsID = b.ID
WHERE a.TypeID = 12 and a.Value like '%wix%' and b.IsActive = 1
ORDER BY b.LastCrawlDate DESC, b.ShopperID



####Maintaining/Monitoring DRONE

DRONE Hardware

Drone is comprised of multiple windows services currently running on 8 BI servers. All 8 servers run an asp.met WebAPI used for collecting the data and reporting on things like "crawl rate".

P3PWSVCWEB001-004 - These servers run Crunchbase, SocialMedia, MarketShare, Scheduler and QueueProcessor services
P3PWBIDATA01-04 - These servers are dedicated to running the CrawlDaddy service

 
####DRONE Tools

Services Dashboard - utilizes SignalR for connections to all 8 servers. Allows start/stop actions and alerts any errors in the system
http://bizintel-ws.intranet.gdg/bidata/home/monitor

Discovery page - Details the existing and planned future API calls utilized by DRONE
http://bizintel-ws.intranet.gdg/bidata/home/discovery 

Other

Example call for domain data (by domain):
http://bizintel-ws.intranet.gdg/bidata/api/portfolio/ATLANTAWEDDINGFLORALS.COM 

Example call for domain data (by ShopperID): 
http://bizintel-ws.intranet.gdg/bidata/api/portfolio/31171465 

Other features of the DRONE Framework

Scheduler - Runs BTEQ scripts agains teradata using a cron style service (quartz.net). 
The database underneath is a MongoDB replicaset hosted on the p3pwbidata0x servers. 
http://bizintel-ws.intranet.gdg/bidata/scheduler

Logi>R webservice - A web api that exposes R functionality for reporting. Returns XML, used by Logi
Example:http://bizintel-ws.intranet.gdg/BIData/api/r.xml?id=GlobalWeekly.r&parms=WeekEnding%7cBusiness%20Registrations%7cUS%7c2
