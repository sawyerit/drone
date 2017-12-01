using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace Drone.WebAPI
{
    public class DynamicPolicyProviderFactory : ICorsPolicyProviderFactory
    {
        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
        {
            var route = request.GetRouteData();
            var corsRequestContext = request.GetCorsRequestContext();
            var policy = GetPolicyForControllerAndOrigin((string)route.Values["controller"], corsRequestContext.Origin);

            return new CustomPolicyProvider(policy);
        }

        private CorsPolicy GetPolicyForControllerAndOrigin(string controller, string originRequested)
        {
            // TODO: database lookup to determine if the controller is allowed for
            // the origin and create CorsPolicy if it is (otherwise return null)
            var policy = new CorsPolicy();
            policy.Origins.Add(originRequested);
            policy.Methods.Add("GET");
            policy.SupportsCredentials = true;
            return policy;
        }
    }
    public class CustomPolicyProvider : ICorsPolicyProvider
    {
        CorsPolicy policy;
        public CustomPolicyProvider(CorsPolicy policy)
        {
            this.policy = policy;
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.policy);
        }
    }
}