using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.API.Dig.Ssl
{
	public class SSLCert
	{
		public SSLCert(System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate, string domainName)
		{
			DomainToCheck = domainName;
			Issuer = x509Certificate.Issuer;
			Subject = x509Certificate.Subject;
			Expires = x509Certificate.GetExpirationDateString();
		}

		public SSLCert()
		{ }

		public string Issuer { get; set; }
		public string IssuerName
		{
			get
			{
				return ParseValueFromCert(Issuer, "CN=");
			}
		}

		public string Subject { get; set; }
		public string SubjectType
		{
			get
			{
				return ParseValueFromCert(Subject, "CN=");
			}
		}

		public string DomainToCheck { get; set; }

		public string FixedName { get; set; }

		public string Expires { get; set; }

		private bool IsValid
		{
			get
			{
				return DateTime.Today < Convert.ToDateTime(Expires) && Subject.ToLower().Contains(DomainToCheck.ToLower());
			}
		}

		private string ParseValueFromCert(string certString, string key)
		{
			string value = "None";
			if (IsValid)
			{
				string[] items = certString.Split(',');

				if (items.Count() > 0)
				{
					foreach (var item in items)
					{
						if (item.Trim().StartsWith(key))
						{
							value = item.Split('=')[1];
							break;
						}
					}
				}
			}

			return value;
		}
	}
}
