using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Interfaces;

namespace Drone.WebAPI.Controllers
{
	public class DomainsController : BaseController
	{
		private readonly IDomainsService _domainService;

		public DomainsController(IDomainsService domainService)
		{
			_domainService = domainService;
		}

		// GET api/domains?count=# (for portfolio domains)
		public List<Domain> Get(int count)
		{
			return _domainService.Get(count);
		}
		
		// GET api/domains?count=#&mask=# 
		//http://localhost:63222/api/domains?count=5&mask=3
		public List<Domain> Get(int count, int mask)
		{
			return _domainService.Get(count, mask);
		}

        /// <summary>
        /// Used for getting domains from MongoDB
        /// </summary>
        /// <param name="count">Number of domains to return</param>
        /// <param name="sourceCollection">MongoDB collection to pull domains from</param>
        /// <returns>List of Domains</returns>
        /// <usage>http://localhost/BIDataAPI/api/domains?count=4&sourceCollection=TestSource</usage>
        public List<Domain> Get(int count, string sourceCollection)
        {
            return _domainService.GetFromMongo(count, sourceCollection);
        }

		// GET api/domains/lookup/domain
		[ActionName("Lookup")]
		public Domain GetDomain(string domain)
		{
			return _domainService.LookupDomain(domain);
		}

		// POST api/domains
		public HttpResponseMessage Post([FromBody]Domain value)
		{
			//todo: do we really want to add a domain to the db to process??  This is not one-off
			HttpResponseMessage response;

			if (!Object.Equals(null, value))
			{
				Domain d = _domainService.Create(value);
				response = Request.CreateResponse(HttpStatusCode.Created);
				response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = d.DomainId }));
			}
			else
			{
				response = Request.CreateResponse(HttpStatusCode.BadRequest);
			}

			return response;
		}				
	}
}