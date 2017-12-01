using System.Collections.Generic;
using System.Web.Http;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Interfaces;
using System;
using System.Net.Http;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Http.Formatting;

namespace Drone.WebAPI.Controllers
{
    public class MiscController : BaseController
    {
        private readonly IMiscService _miscService;

        public MiscController(IMiscService service)
        {
            _miscService = service;
        }

        [JsonpFilter]
        [System.Web.Mvc.HttpPost]
        public JsonResult TDCompare(FormDataCollection form)
        {
            return new JsonResult
            {
                Data = _miscService.CompareQueries(form.Get("q1text"), form.Get("q2text"), form.Get("usertext")),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}