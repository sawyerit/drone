using Drone.WebAPI.Controllers;
using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Drone.WebAPI
{
    public class RequestLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            if (actionContext.ActionDescriptor.GetCustomAttributes<NoLogAttribute>().Any()
                || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoLogAttribute>().Any())
                return;

            BaseController bc = actionContext.ControllerContext.Controller as BaseController;
            if (!Object.Equals(null, bc))
                bc.IncrementCount();

            //todo: maybe log the call as well
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class NoLogAttribute : Attribute
    {
        //use this attr to not log the call?
        string s = "Test";
    }
}