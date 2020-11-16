using BusinessLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class SubjectController : ApiController
    {
        private readonly SubjectBusiness subjectBusiness;

        public SubjectController()
        {
            subjectBusiness = new SubjectBusiness();
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get the List<Subject>
            List<Subject> subjects = subjectBusiness.GetAllSubjects();

            //Check if the List<Subject> is not empty
            if (subjects.Count() > 0)
            {
                //Return 200 + subjects
                return Request.CreateResponse(HttpStatusCode.OK, subjects);
            }
            else
            {
                //Return 404 + string with message
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Subjects Found!");
            }
        }
    }
}
