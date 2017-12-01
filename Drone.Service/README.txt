**NOTES** 

- The user must be specified in the nimitz cert to have access to the database (network service does??)

- The service will attempt to wait for all threads to complete before stopping, however, some long running threads may still be processing, 
so be sure the process is gone from task manager before copying new dlls

- The main application is coded using MEF, so the dll's marked for export are loaded dynamically and consumed via adhering to an interface.
This means when deploying, any dlls containing components to process must be manually deployed as well as any xml files they use

- The webservice project is used to expose the data collection methods for one-time usage (to get missed data etc.)


**TROUBLESHOOTING**

- Service account has correct permissions?
- StoredProcedure and component xml files deployed?
- XML files properly set time intervals and IsEnabled property?
- Nimitz entries in service.exe.config file?
- Webservice endpoints configured in config file?
- Check event log for start/stop and exceptions
- Check BILogViewer for errors in "Drone Processor"
- Run unit tests
- If you can't get whois data, make sure the IP address is allowed to hit our internal lookup service

**Parts**
-Twitter
-Facebook
-SmallBusinessTracking
-QueueManager (MSMQ for database inserting)
-Data (storedproc.xml)
-APIs: Crunchbase, Dig, Facebook, Twitter


**Deployment details**
- Always build and run unit tests, make sure they pass.  The BIN for this project is where the deployment batches scoop files from
- Change config to use prod BILoggingService and prod WhoIs URL
- Make sure XML Files for all components, stored procs, name-lookups (for dig) are deployed
- CrunchbaseComponent.XML : startfrom value is no longer used. it should auto-recover using the Crunchbase_allcompanies.txt file.





//inflated JSON domain object
[{
    "ShopperID": 31171465,
    "PrivateLabelID": 1,
    "DomainName": "atlantaweddingflorals.com",
    "LastCrawlDate": "2014-02-05T01:41:20.447",
    "Attributes": {
        "WebHost": "CyrusOne LLC",
        "EmailHost": "ezot.com",
        "DNSHost": "bizsiteservice.com",
        "IPaddress": "69.7.179.236",
        "SiteBuilder":"1and1",
        "IsParked":"Static Parking",
        "MailProvider":"ConstantContact",
        "SSLIssuer":"VeriSign",
        "CertificateType":"www.formulx.com",
        "Ecommerce":"zencart"
    },
    "Verticals": {
        "verticalCategoryId": "1229",
        "verticalCategoryName": "custom handmade corsages",
        "verticalKeyword": "corsages",
        "localCity": "atlanta"
    },
    "Social": {
        "facebook": "http://www.facebook.com/pages/Atlanta-Wedding-FloralsSimple-Elegance-Florals/164708760310531",
        "linkedin": "http://www.linkedin.com/asdasdasdasd",
        "twitter": "http://www.twitter.com/asdasdasdasd",
    },
    "TypeId": 0,
    "rptGDDomainsId": 6763971,
    "DomainID": 105897811,
    "UniqueID": "00000000-0000-0000-0000-000000000000"
}]