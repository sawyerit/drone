using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drone.API.Crunchbase;
using Drone.Entities.Crunchbase;
using Drone.Data.Queue;
using Drone.Shared;
using System.Xml;
using System.Messaging;
using Drone.API.Dig;
using Drone.Crunchbase.Datasources;
using Drone.Crunchbase.Components;
using Drone.WebAPI.Services;

namespace Drone.Test
{
	[TestClass]
	public class CrunchbaseTests
	{
		CompanyManager compManager = new CompanyManager();

		[TestInitialize]
		public void Setup()
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crunchbase"); 
		}

		[TestMethod]
		public void GetAllCompanies()
		{
			List<Company> list = compManager.GetAllCompanies();

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
		}

		[TestMethod]
		public void GetAllCompaniesPaged()
		{
			List<Company> list = compManager.GetAllCompaniesPaged();

			Assert.IsNotNull(list);
			Assert.IsTrue(list.Count > 0);
		}

		[TestMethod]
		public void GetCompanyByIndexAndInsertQueueViaAPI()
		{
			List<Company> list = compManager.GetAllCompanies();
			int index = list.FindIndex(item => item.permalink == "wetpaint");

			Crunch crunch = new Crunch(new CrunchbaseDataSource());
			Company co = list.ElementAt(index);
			CompanyRoot cr = crunch.GetFullCompany(co.permalink, new Dig());

			CrunchbaseDataComponent cdc = new CrunchbaseDataComponent();
			cdc.CompanyLocal = cr;
			crunch.DroneDataSource.Process(cdc);

			Assert.IsNotNull(cr);
		}

		[TestMethod]
		public void GetFullCompanyByPermaLinkAndInsertViaDirectCall()
		{
			//@-news.info
			Crunch crunch = new Crunch();
			CrunchbaseDataComponent _dataComponent = new CrunchbaseDataComponent();

            _dataComponent.CompanyLocal = crunch.GetFullCompany("i-App-Creation", new Dig());

            Drone.QueueProcessor.Datasources.CrunchbaseDataSource cds = new Drone.QueueProcessor.Datasources.CrunchbaseDataSource();
            cds.Process(_dataComponent);

			Assert.IsNotNull(_dataComponent.CompanyLocal);
		}

        [TestMethod]
        public void GetFullCompanyByPermaLinkAndInsertQviaAPI()
        {
            //@-news.info
            Crunch crunch = new Crunch();
            CrunchbaseDataComponent _dataComponent = new CrunchbaseDataComponent();

            _dataComponent.CompanyLocal = crunch.GetFullCompany("i-App-Creation", new Dig());

            crunch.DroneDataSource = new CrunchbaseDataSource();
            crunch.DroneDataSource.Process(_dataComponent);

            Assert.IsNotNull(_dataComponent.CompanyLocal);
        }

		[TestMethod]
		public void GetFullCompanyByPermalink()
		{
			//@-news.info
			Crunch crunch = new Crunch();
			CrunchbaseDataComponent _dataComponent = new CrunchbaseDataComponent();
            _dataComponent.CompanyLocal = crunch.GetFullCompany("What-Is-Your-Monster", new Dig());

			Assert.IsNotNull(_dataComponent.CompanyLocal);
			//Assert.AreEqual("Directi Internet Solutions", cr.records.Registrar);
		}

		[TestMethod]
		public void SetNextRunInterval_BaseClass()
		{
			Crunch crunch = new Crunch(new CrunchbaseTestDataSource());
			crunch.SetNextRunIntervalByNode("crunchbase", crunch.Context);

			Assert.AreEqual(crunch.Context.NextRun, DateTime.Now.Next((DayOfWeek)Enum.Parse(typeof(DayOfWeek), "saturday", true)).Date);
		}

		[TestMethod]
		public void WriteToLogFile()
		{
			Utility.WriteToLogFile("testFile.log","test message");
		}

		[TestMethod]
		public void GetData()
		{
			Crunch crunch = new Crunch(new CrunchbaseDataSource());

			crunch.GetData(crunch.Context);
		}

		[TestMethod]
		public void GetAllCompaniesCrunchbase()
		{
			Crunch crunch = new Crunch();
			crunch.DroneDataSource = new CrunchbaseTestDataSource();

			crunch.GetAllCompanies();
		}

		[TestMethod]
		public void CleanUrl()
		{
			Crunch crunch = new Crunch();
			
			//htttp
			string newURL = Utility.CleanUrl("htttp://aprendelo.com");
			Assert.IsTrue(newURL == "aprendelo.com");

			//case and trailing /
			newURL = Utility.CleanUrl("case.syr.edu/incubators/incubator.php");
			Assert.IsTrue(newURL == "syr.edu");

			//about.
			newURL = Utility.CleanUrl("about.picsearch.com");
			Assert.IsTrue(newURL == "picsearch.com");

			//about.
			newURL = Utility.CleanUrl("about.com");
			Assert.IsTrue(newURL == "about.com");

			//beta.
			newURL = Utility.CleanUrl("beta.lt");
			Assert.IsTrue(newURL == "beta.lt");

			//beta.
			newURL = Utility.CleanUrl("beta.booklamp.org");
			Assert.IsTrue(newURL == "booklamp.org");

			//global.
			newURL = Utility.CleanUrl("global.bose.com/index.html");
			Assert.IsTrue(newURL == "bose.com");

			//ir.
			newURL = Utility.CleanUrl("ir.dangdang.com");
			Assert.IsTrue(newURL == "dangdang.com");
			
		}

		[TestMethod]
		public void ExceptionInformationLogging()
		{
			ExceptionExtensions.LogInformation("SmallBusinessTrackingTests.ExceptionInformationLogging", "Logging information test");
		}

		[TestMethod]
		public void SkipLoop_Test()
		{
			
			IList<int> mainList = new List<int>();

			mainList = Enumerable.Range(1, 97561).ToList();

			int iterationbase = 1000;

			for (int i = 0; i < mainList.Count; i+=iterationbase)
			{
				List<int> subList = mainList.Skip(i).Take(iterationbase).ToList();
			}
		
		}

		[TestMethod]
		[Ignore]
		public void GenericTest()
		{
			//List<string> listy = new List<string>();
			//listy.Add("Zebra");
			//listy.Add("cow");
			//listy.Add("moose");

			//listy = listy.OrderBy(item => item).ToList();

			//object i = Enum.Parse(Type.GetType("DayOfWeek", false), "sunday");
			DateTime dt = DateTime.Now.Next((DayOfWeek)Enum.Parse(typeof(DayOfWeek), "Sunday"));		
		}

		[TestMethod]
		[Ignore]
		public void DateTest()
		{
			DateTime dt = new DateTime(2010, 1, 15);

		}

		[TestMethod]
		[Ignore]
		public void GetFoundedDate()
		{
			//get all entries with 1/1/1900, name and domain name

			List<string[]> entries = GetAll1900s();

			Crunch crunch = new Crunch(new CrunchbaseTestDataSource());
			List<Company> _allCompanies = new CompanyManager().GetAllCompanies();

			foreach (string[] item in entries)
			{
				try
				{
					Company co = _allCompanies.Where(com => com.name.Trim().ToLowerInvariant() == item[1].Trim().ToLowerInvariant()).FirstOrDefault();
					if (!Object.Equals(co, null))
					{
						CompanyRoot cor = crunch.GetFullCompany(co.permalink, new Dig());
						if (!Object.Equals(cor, null))
						{
							cor.homepage_url = cor.homepage_url.Replace("http://", "").Replace("https://", "").Replace("www.", "").TrimEnd(new char[] { '/' });

							if (cor.homepage_url == item[0])
							{
								CrunchbaseDataSource ds = new CrunchbaseDataSource();
								CrunchbaseDataComponent _dataComponent = crunch.DroneDataComponent as CrunchbaseDataComponent;

								if (cor.founded_year != null || cor.founded_month != null || cor.founded_day != null)
								{
									_dataComponent.CompanyLocal = cor;
									ds.Process(_dataComponent);
								}
							}
						}
					}
					else
					{
						//name lookup failed. 

					}
				}
				catch (Exception)
				{ }
			}

		}

		private List<string[]> GetAll1900s()
		{

			List<string[]> parsedData = new List<string[]>();

			try
			{
				//string sXMLPath = Path.Combine(Utility.ComponentBaseFolder, ComponentTypeString + ".xml");
				using (StreamReader readFile = new StreamReader(Path.Combine(Utility.ComponentBaseFolder, "all_1900s.csv")))
				{
					string line;
					string[] row;

					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(new char[] { ',' }, 2);
						parsedData.Add(row);
					}
				}
			}
			catch (Exception) { }

			return parsedData;

		}
	}
}
