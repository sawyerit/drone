
using System;
namespace Drone.Entities.MarketShare
{
	public enum MarketShareDataTypeEnum
	{
		CompanyName = 1,
		DateFounded = 2,
		WebHost = 3,
		EmailHost = 4,
		DNSHost = 5,
		Registrar = 6,
		SSLIssuer = 7,
		CertificateType = 8,
		NumberOfEmployees = 9,
		TotalFunding = 10,
		Investors = 11,
		SiteBuilder = 12,
		Ecommerce = 13,
		Verticals = 14,
		Social = 15,
		HasCart = 16,
		HasLogin = 17,
		PiiForm = 18,
		MailProvider = 19,
        IsParked = 20,
        IPaddress = 21
	}

	[Flags]
	public enum MarketShareTypeBitMaskEnum
	{
		CompanyName = 1,
		DateFounded = 2,
		WebHost = 4,
		EmailHost = 4,
		DNSHost = 16,
		Registrar = 32,
		SSLIssuer = 64,
		CertificateType = 128,
		NumberOfEmployees = 256,
		TotalFunding = 512,
		Investors = 1024,
		SiteBuilder = 2048,
		Ecommerce = 4096
	}
}
