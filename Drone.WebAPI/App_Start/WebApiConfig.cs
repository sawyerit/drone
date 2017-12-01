using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;

namespace Drone.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var cors = new EnableCorsAttribute("http://localhost:56853", "*", "*"); //specify all hosts here
            //cors.SupportsCredentials = true;
            //config.EnableCors(cors);
            config.EnableCors();
            config.SetCorsPolicyProviderFactory(new DynamicPolicyProviderFactory());
            //remove entries from webconfig
            //install PM> Install-Package Microsoft.AspNet.WebApi.Cors
            //set web.api dll statup.cs to enable cors again (for signalr)
            //get rid of withCredential:false in the signalr js start()
            //deploy this and try on bidata01 with authentication turned on
            //http://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api#enable-cors
            //add this to any cross domain calling client, in the ajax request (at least for chrome to work)
            //xhrFields: { withCredentials: true },

            //channel/channelname route for youtube
            config.Routes.MapHttpRoute(
                    name: "YouTubeActionApi",
                    routeTemplate: "api/{controller}/{action}/{user}",
                    defaults: new { user = RouteParameter.Optional },
                    constraints: new { controller = "youtube", action = "Channel" } //required because youtube/id needs to fall through to default
            );

            //route for twitter keywords or mentions
            //a new route would be required to have api/twitter/mentions/{paged}/id.  for now, its ?=param
            config.Routes.MapHttpRoute(
                    name: "GetAllActionApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                    constraints: new { controller = "twitter" } //no action needed here because twitter is ALWAYS called with an action included
            );

            //for facebook posts
            config.Routes.MapHttpRoute(
                    name: "FacebookPostApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                    constraints: new { controller = "facebook", action = "Page|Country|Gender|Locale" } //adding the action restraint here allows facebook/id calls to pass through to default
            );

            //route for common
            config.Routes.MapHttpRoute(
                    name: "GetCommonActionApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                    constraints: new { controller = "common" } //the controler must be specified so it doesn't catch facebook/id or crunchbase/id and try to make the id the action.
            );

            //route for portfolio
            config.Routes.MapHttpRoute(
                    name: "GetPortfolioActionApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                    constraints: new { controller = "portfolio" } //the controler must be specified so it doesn't catch facebook/id or crunchbase/id and try to make the id the action.
            );

            //domain lookup
            config.Routes.MapHttpRoute(
                    name: "DomainActionApi",
                    routeTemplate: "api/{controller}/{action}/{domain}",
                    defaults: new { action = "lookup" },
                    constraints: new { controller = "domains", action = "lookup" }
            );

            // R route for .xml extension
            config.Routes.MapHttpRoute(
                   name: "Api UriPathExtension",
                   routeTemplate: "api/{controller}.{ext}",
                   defaults: new { controller = "R", extension = RouteParameter.Optional }
            );

            //default route
            config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "text/xml");
        }
    }
}
