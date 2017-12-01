using Drone.WebAPI.Interfaces;
using System.Web.Http;
using System.Xml.Linq;

namespace Drone.WebAPI.Controllers
{
    public class RController : BaseController
    {
        private readonly IRService _rService;

		public RController(IRService rService)
		{
			_rService = rService;
		}


        // GET api/r
        //http://localhost:63222/api/r.xml?id=myscript2.r&parms=testarg1%20testarg2
        public XElement Get(string id, string parms)
        {
            return _rService.Get(id, parms);
        }

        public XElement Get(string id)
        {
            return _rService.Get(id);
        }
    }
}
