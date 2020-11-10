using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class AssignmentController : ApiController
    {

        //private assignmentInterface class

        public AssignmentController()
        {
            //instantiate assignmentInterface class
        }

        //[Route("Assignment")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //assignmentInterface getAll
            //return in HttpResonseMessage body List<Assignment>
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            return httpResponseMessage;
        }


        //[Route("Assignment/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            return httpResponseMessage;
        }


        //[Route("Assignment")]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            //assignmentInterface create
            //return in HttpResonseMessage body Assignment
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            return httpResponseMessage;

        }


        //[Route("Assignment/{id}")]
        [HttpPost]
        public HttpResponseMessage Post(int id)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);

        }

        [Route("Assignment")]
        [HttpPut]
        public HttpResponseMessage Put()
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("Assignment/{id}")]
        [HttpPut]
        public HttpResponseMessage Put(int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            return httpResponseMessage;
        }

        [Route("Assignment")]
        [HttpDelete]
        public HttpResponseMessage Delete()
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }


        [Route("Assignment/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            //assignmentInterface delete
            //return if operation was successful
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            return httpResponseMessage;
        }


    }
}
