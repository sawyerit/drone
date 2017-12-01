using System;
using System.Collections.Generic;
using System.Data;
using Drone.Data;
using Drone.Entities.Crunchbase;
using Drone.Shared;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Services
{
	public class CrunchbaseService : BaseService, ICrunchbaseService
	{
		public CompanyRoot Create(CompanyRoot value)
		{
			try
			{
				_queueManager.AddToQueue(Utility.SerializeToXMLString<CompanyRoot>(value), "Crunchbase " + value.permalink);
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "CrunchbaseService.Create", value.permalink);
			}
			return value;
		}

		public List<Company> GetAll()
		{
			List<Company> companyList = new List<Company>();
			return companyList;
		}

		public List<Company> GetPaged(int page)
		{
			List<Company> companyList = new List<Company>();
			return companyList;
		}

		public CompanyRoot Get(string domain)
		{
			CompanyRoot cr = null;
			DataSet ds;
			try
			{
				ds = DataFactory.GetDataSetByName("CrunchbaseGetByDomain", new KeyValuePair<string, object>("@DomainName", domain));
				
				if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
				{
					cr = new CompanyRoot();
					DataRow dr = ds.Tables[0].Rows[0];
					cr.homepage_url = dr["Domain"].ToString();
					cr.name = dr["CompanyName"].ToString();
					cr.permalink = cr.name.Replace(" ", "-");
					cr.crunchbase_url = "http://api.crunchbase.com/v/1/company/" + cr.permalink + ".js";

					DateTime dateFounded = Convert.ToDateTime(dr["DateFounded"]);
					cr.founded_day = dateFounded.Day;
					cr.founded_month = dateFounded.Month;
					cr.founded_year = dateFounded.Year;

					cr.records = new Records {  WebHost = dr["WebHost"].ToString()
																		, DNSHost = dr["DNSHost"].ToString()
																		, EmailHost = dr["EmailHost"].ToString()
																		, SSLIssuer = dr["SSLIssuer"].ToString()
																		, CertificateType = dr["CertificateType"].ToString()
																		, Registrar = dr["Registrar"].ToString() };
					cr.number_of_employees = dr["NumberOfEmployees"].ToString().ConvertStringToInt(0);
					cr.total_money_raised = dr["TotalFunding"].ToString();
				}
			}
			catch (Exception)
			{
				//todo: log error
			}
			
			return cr;
		}
	}
}