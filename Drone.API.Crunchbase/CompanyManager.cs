using System;
using System.Collections.Generic;
using System.Threading;
using Drone.Entities.Crunchbase;
using Drone.Shared;
using System.Linq;

namespace Drone.API.Crunchbase
{
	public class CompanyManager : BaseManager
	{
		/// <summary>
		/// Get all companies from Crunchbase with retry if it fails
		/// </summary>
		/// <param name="retry"></param>
		/// <returns></returns>
		public List<Company> GetAllCompanies()
		{
			List<Company> allCompanies = new List<Company>();
			string requestText = string.Format("{0}/companies.js?api_key={1}", ApiUrl, ApiKey);
			List<Company> companyList = new List<Company>();
			int retry = 0;

			while ((Object.Equals(null, companyList) || companyList.Count <= 0))
			{
				try
				{
					companyList = Request.Deserialize<List<Company>>(Request.ExecuteAnonymousWebRequest(requestText), VerboseLogging);
					if (Object.Equals(null, companyList) || companyList.Count <= 0)
					{
						Thread.Sleep(120000);
					}
				}
				catch (Exception e)
				{
					if (retry < 10)
					{
						ExceptionExtensions.LogInformation("Crunchbase.CompanyManager.GetAllCompanies", "GetAllCompanies() failed with exception, trying again. ");
						Thread.Sleep(120000);
						++retry;
					}
					else
					{
						throw;
					}
				}
			}

			if (!Object.Equals(null, companyList))
				allCompanies.AddRange(companyList);

			return allCompanies;
		}

		public List<Company> GetAllCompaniesPaged()
		{
			List<Company> allCompanies = new List<Company>();
			List<Company> companyList = new List<Company>();
			string requestText = string.Empty; int page = 0; int retry = 0;

			while (true)
			{
				try
				{
					requestText = string.Format("{0}/companies.js?page={2}&api_key={1}", ApiUrl, ApiKey, page);
					companyList = Request.Deserialize<List<Company>>(Request.ExecuteAnonymousWebRequest(requestText), VerboseLogging);

					if (!Object.Equals(null, companyList))
					{
						if (page > (companyList[0].total / companyList[0].per_page))
						{
							break;
						}
						else
						{
							allCompanies.AddRange(companyList.Skip(1));
							page++;
						}
					}
					else
					{
						if (retry < 10)
						{
							Thread.Sleep(30000);
							retry++;
						}
						else
						{
							throw new ArgumentNullException("GetAllCompaniesPaged returned NULL 10 times");
						}
					}
				}
				catch (Exception e)
				{
					if (retry < 10)
					{
						ExceptionExtensions.LogInformation("Crunchbase.CompanyManager.GetAllCompaniesPaged", "GetAllCompaniesPaged() failed with exception, trying again. ");
						Thread.Sleep(30000);
						retry++;
					}
					else
					{
						throw;
					}
				}
			}

			return allCompanies;
		}

		/// <summary>
		/// Get full company from crunchbase w/retry if it fails
		/// </summary>
		/// <param name="permalinkName"></param>
		/// <param name="retry"></param>
		/// <returns></returns>
		public CompanyRoot GetFullCompany(string permalinkName, int retry = 0)
		{
			string requestText = string.Format("{1}/company/{0}.js?api_key={2}", permalinkName, ApiUrl, ApiKey);
			CompanyRoot cr = new CompanyRoot();

			try
			{
				cr = Request.Deserialize<CompanyRoot>(Request.ExecuteAnonymousWebRequest(requestText), VerboseLogging);
				if (Object.Equals(cr, null))
					RetryFullCompany(permalinkName, retry);
			}
			catch (Exception)
			{
				RetryFullCompany(permalinkName, retry);
			}

			return cr;
		}

		private void RetryFullCompany(string permalinkName, int retry)
		{
			if (retry < 1)
			{
				Thread.Sleep(2000);
				GetFullCompany(permalinkName, ++retry);
			}
			else
			{
				if (VerboseLogging)
					Utility.WriteToLogFile(String.Format("Crunchbase_CompanyRetry_{0:M_d_yyyy}", DateTime.Today) + ".log", "permalink: " + permalinkName);
			}
		}

	}
}
