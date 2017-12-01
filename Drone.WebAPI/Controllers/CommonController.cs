using System.Collections.Generic;
using System.Web.Http;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Interfaces;
using System;
using System.Net.Http;
using System.Net;

namespace Drone.WebAPI.Controllers
{
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;

        public CommonController(ICommonService service)
        {
            _commonService = service;
        }

        #region GETS

        // GET api/common/competitors
        [ActionName("Competitors")]
        public List<Competitor> GetCompetitors()
        {
            return _commonService.GetCompetitors();
        }

        // GET api/common/competitors/id
        [ActionName("Competitors")]
        public Competitor GetCompetitors(int id)
        {
            return _commonService.GetCompetitor(id);
        }

        // GET api/common/peek/count
        [ActionName("Peek")]
        public List<string> GetMSMQPeek(int count)
        {
            List<string> n = _commonService.PeekQueue(count);            
            return n;
        }

        #endregion GETS

        #region POSTS
        // POST api/common/domaincomplete
        //[NoLog()]  use this if you dont want this call to count in the api requests gauge
        [ActionName("DomainComplete")]
        public HttpResponseMessage DomainComplete(dynamic value)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            if (!Object.Equals(null, value))
            {
                //string name = value.Name;
                //int domainComplete = value.DomainComplete;
                //int pages = value.Pages;
                IncrementCrawlDaddyCount();
                response = Request.CreateResponse(HttpStatusCode.OK);
            }

            return response;
        }

        #endregion
    }
}