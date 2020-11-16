using BusinessLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class AcademicLevelController : ApiController
    {
        private readonly AcademicLevelBusiness alb;

        public AcademicLevelController()
        {
            alb = new AcademicLevelBusiness();
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get the List<AcademicLevel>
            List<AcademicLevel> levels = alb.GetAllAcademicLevels();

            //Check if the List<AcademicLevel> is not empty
            if (levels.Count() > 0)
            {
                //Return 200 + levels
                return Request.CreateResponse(HttpStatusCode.OK, levels);
            }
            else
            {
                //Return 404 + string with message
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Academic levels Found!");
            }
        }
    }
}
