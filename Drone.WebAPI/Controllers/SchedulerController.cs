using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using Drone.Entities.WebAPI;
using Drone.WebAPI.Models;
using Drone.WebAPI.Services;
using Drone.Scheduler;
using System.Web;

namespace Drone.WebAPI.Controllers
{
    public class SchedulerController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.FullName = System.Web.HttpContext.Current.User.Identity.Name;
            List<Job> allJobs = SchedulerService.GetAllJobs();
            return View(allJobs);
        }

        /// <summary>
        /// Run job now.  Spins up a new scheduler and triggers the job immediately
        /// </summary>
        /// <param name="id">Job's Mongo ObjectId</param>
        /// <returns>Job summary data</returns>
        public JsonResult RunJob(string id)
        {
            return new JsonResult
            {
                Data = SchedulerService.RunJob(id),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult EnableDisableJob(string id)
        {
            return new JsonResult
            {
                Data = SchedulerService.EnableDisableJob(id),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNewJob(FormCollection formValues)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                SchedulerService.SaveSharedFile(Request.Files[i], Request["projnametb"]);
            }

            bool wasSaved = SchedulerService.CreateNewJob(Request["jobidtb"], Request["jobtypedd"], Request["projnametb"], Request["scriptdd"], Request["isactivecb"], Request["starttimetb"], Request["cronxtb"]);
            if (wasSaved)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Error");
        }

        /// <summary>
        /// Job Summary
        /// </summary>
        /// <returns>Job summary data</returns>
        public JsonResult GetSummary()
        {
            return new JsonResult
            {
                Data = SchedulerService.GetAllJobs(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// Job Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Job detail data expanded</returns>
        public JsonResult GetDetails(string id)
        {
            return new JsonResult
            {
                Data = SchedulerService.GetJob(id),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetSchedule(string cron, string startdatetime)
        {
            SchedulerService ss = new SchedulerService();
            return new JsonResult
            {
                Data = ss.GetNextFive(cron, startdatetime),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
