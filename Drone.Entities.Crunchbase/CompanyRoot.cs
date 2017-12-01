using System.Collections.Generic;
using System;

namespace Drone.Entities.Crunchbase
{

	public class Person
	{
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string permalink { get; set; }
	}

	public class FinancialOrg
	{
		public string name { get; set; }
		public string permalink { get; set; }
	}

	public class Investment
	{
		public Company company { get; set; }
		public FinancialOrg financial_org { get; set; }
		public Person person { get; set; }
	}

	public class FundingRound
	{
		public string round_code { get; set; }
		public string source_url { get; set; }
		public string source_description { get; set; }
		public double? raised_amount { get; set; }
		public string raised_currency_code { get; set; }
		public int? funded_year { get; set; }
		public int? funded_month { get; set; }
		public int? funded_day { get; set; }
		public List<Investment> investments { get; set; }
	}

	public class Records
	{
		public string WebHost { get; set; }
		public string EmailHost { get; set; }
		public string DNSHost { get; set; }
		public string Registrar { get; set; }
		public string SSLIssuer { get; set; }
		public string CertificateType { get; set; }
	}

	public class CompanyRoot
	{
		public string name { get; set; }
		public string permalink { get; set; }
		public string crunchbase_url { get; set; }
		public string homepage_url { get; set; }
		public string ip_address { get; set; }
		public string blog_url { get; set; }
		public string blog_feed_url { get; set; }
		public string twitter_username { get; set; }
		public string category_code { get; set; }
		public int? number_of_employees { get; set; }
		public int? founded_year { get; set; }
		public int? founded_month { get; set; }
		public int? founded_day { get; set; }
		public string email_address { get; set; }
		public string phone_number { get; set; }
		public string description { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		public string overview { get; set; }
		public string total_money_raised { get; set; }
		public List<FundingRound> funding_rounds { get; set; }
		public Records records { get; set; }
	}
}
