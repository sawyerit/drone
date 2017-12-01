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

namespace Drone.WebAPI.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
            ViewBag.FullName = System.Web.HttpContext.Current.User.Identity.Name;
			return View();
		}

		public ActionResult Discovery()
		{
            ViewBag.FullName = System.Web.HttpContext.Current.User.Identity.Name;
			return View(GetAPIDocumentation());
		}

        public ActionResult Monitor()
        {
            ViewBag.FullName = System.Web.HttpContext.Current.User.Identity.Name;
            return View();
        }

        //not used, but leave for reference
        [JsonpFilter]
        public JsonResult RequestCount(string callback)
        {
            return new JsonResult
            {
                Data = new { RequestInfo = string.Format("{0} - {1}", BaseController.CurrentRequestCount, BaseController.RequestCountLastReset), ServerName = System.Environment.MachineName },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

		public JsonResult CauseError(string message)
		{
			CommonService cs = new CommonService();

			cs.WriteError(message);
			return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		private Dictionary<string, List<API>> GetAPIDocumentation()
		{

			Dictionary<string, List<API>> apiDictionary = new Dictionary<string, List<API>>();

			XmlDocument xmlDoc = new XmlDocument();
			string sXMLPath = Path.Combine(Server.MapPath("~/app_data"), "APIDocumentation.xml");

			try
			{
				xmlDoc.Load(sXMLPath);
				XmlNodeList xnList = xmlDoc.SelectNodes("/Documentation/WebAPI/API");

				foreach (XmlNode node in xnList)
				{
					if (!apiDictionary.ContainsKey(node["Category"].InnerText))
					{
						List<API> newList = new List<API>();
						apiDictionary.Add(node["Category"].InnerText, newList);
					}

					API api = new API();
					api.Type = (APIType)Enum.Parse(typeof(APIType), node["Type"].InnerText);
					api.Category = node["Category"].InnerText;
					api.Documentation = node["Description"].InnerText;
					api.Url = node["RelativeUrl"].InnerText;
					api.UsageExample = node["Usage"].InnerText;


					apiDictionary[node["Category"].InnerText].Add(api);
				}
			}
			catch (Exception e)
			{
			}

			return apiDictionary;
		}
	}
}
