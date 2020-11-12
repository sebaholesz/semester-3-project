using BusinessLayer;
using ModelLayer;
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

        private AssignmentBusiness assignmentBusiness;

        public AssignmentController()
        {
            assignmentBusiness = new AssignmentBusiness();
        }

        //[Route("Assignment")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get the List<Assignments>
            List<Assignment> assignments = assignmentBusiness.GetAllAssignments();

            //Check if the List<Assignments> is not empty
            if (assignments.Count() > 0)
            {
                //Return 200 + assignments
                return Request.CreateResponse(HttpStatusCode.OK, assignments);
            }
            else
            {
                //Return 404 + string with message
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Assigments Found");
            }
        }


        //[Route("Assignment/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment

            Assignment assignment =  assignmentBusiness.GetByAssignmentId(id);
            return  assignment != null ? Request.CreateResponse(HttpStatusCode.OK, assignment) : Request.CreateResponse(HttpStatusCode.NotFound);
        }


        //[Route("Assignment")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Assignment assignment)
        {
            //Attempt the creation of the assignment and save the bool value of the result
            //bool wasSuccesful = assignmentBusiness.CreateAssignment(assignment);
            int rowsAffected = assignmentBusiness.CreateAssignment(assignment);

            //Check if the creation was successful
            if (rowsAffected > 0)
            {
                //Return 201 + string with message
                return Request.CreateResponse(HttpStatusCode.Created, "Assignment Created Successfuly");
            }
            else
            {
                //Return 409 + string with message
                return Request.CreateResponse(HttpStatusCode.NotFound, "Assignment Creation Failed");
            }
        }


        //[Route("Assignment/{id}")]
        [HttpPost]
        public HttpResponseMessage Post(int id)
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request");
        }


        //[Route("Assignment")]
        [HttpPut]
        public HttpResponseMessage Put()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request");
        }


        //[Route("Assignment/{id}")]
        [HttpPut]
        public HttpResponseMessage Put([FromBody] Assignment assignment, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = assignmentBusiness.UpdateAssignment(assignment, id);
            return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        //[Route("Assignment")]
        [HttpDelete]
        public HttpResponseMessage Delete()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request");
        }


        //[Route("Assignment/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            //assignmentInterface delete
            //return if operation was successful

            int noOfRows = assignmentBusiness.DeleteAssignment(id);
            return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}
