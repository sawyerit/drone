using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using System.Web.Http;
using Drone.WebAPI.Interfaces;
using Drone.WebAPI.Services;

namespace Drone.WebAPI
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ICrunchbaseService, CrunchbaseService>();
            container.RegisterType<IFacebookService, FacebookService>();
            container.RegisterType<IMarketShareService, MarketShareService>();
            container.RegisterType<ITwitterService, TwitterService>();
            container.RegisterType<IYouTubeService, YouTubeService>();
            container.RegisterType<IDomainsService, DomainsService>();
            container.RegisterType<ICommonService, CommonService>();
            container.RegisterType<IPortfolioService, PortfolioService>();
            container.RegisterType<IRService, RService>();
            container.RegisterType<IMiscService, MiscService>();
        }
    }
}