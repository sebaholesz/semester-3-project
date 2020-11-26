using BusinessLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Route("apiV1/")]
    public class APISolutionController : ApiController
    {
        private readonly SolutionBusiness solutionBusiness;

        public APISolutionController()
        {
            solutionBusiness = new SolutionBusiness();
        }

        [Route("solution")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get the List<Assignments>
            List<Solution> solutions = solutionBusiness.GetAllSolutions();

            //Check if the List<Assignments> is not empty and return 200 + solutions if true else return 404 + messasge
            return solutions.Count > 0 ? Request.CreateResponse(HttpStatusCode.OK, solutions) : Request.CreateResponse(HttpStatusCode.NotFound, "No Solutions found!");
        }

        [Route("solution/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment

            Solution solution = solutionBusiness.GetBySolutionId(id);
            return solution != null ? Request.CreateResponse(HttpStatusCode.OK, solution) : Request.CreateResponse(HttpStatusCode.NotFound, "Solution with that ID not found!");
        }

        [Route("solution/byAssignmentId/{assignmentId}")]
        [HttpGet]
        public HttpResponseMessage GetAllSolutionByAssignmentId(int assignmentId)
        {
            //the list is oredered by timestamp

            List<Solution> solutions = solutionBusiness.GetSolutionsTimestampOrderedByAssignmentId(assignmentId);
            return solutions.Count > 0 ? Request.CreateResponse(HttpStatusCode.OK, solutions) : Request.CreateResponse(HttpStatusCode.NotFound, "Solutions with that AssignmentID not found!");
        }

        [Route("solution")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Solution solution)
        {
            //Attempt the creation of the solutioin and save the int value of the rows affected
            int queuePosition = solutionBusiness.CreateSolution(solution);

            //Check if the creation was successful and return 201 + queue position or 409 + -1 as a fail message
            return queuePosition >= 0 ? Request.CreateResponse(HttpStatusCode.Created, queuePosition) : Request.CreateResponse(HttpStatusCode.Conflict, queuePosition);
        }

        [Route("solution/{id}")]
        [HttpPost]
        public HttpResponseMessage Post(int id)
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }

        [Route("solution/{id}")]
        [HttpPut]
        public HttpResponseMessage Put([FromBody] Solution solution, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = solutionBusiness.UpdateSolution(solution, id);
            return noOfRows > 0 ? Request.CreateResponse(HttpStatusCode.OK, "Solution Updated Successfuly!") : Request.CreateResponse(HttpStatusCode.NotFound, "Solution was not found!");
        }

        [Route("solution")]
        [HttpPut]
        public HttpResponseMessage Put()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }

        [Route("solution/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            //return if operation was successful

            int noOfRows = solutionBusiness.DeleteSolution(id);
            return noOfRows > 0 ? Request.CreateResponse(HttpStatusCode.OK, "Solution Deleted Successfuly!") : Request.CreateResponse(HttpStatusCode.NotFound, "Solution was not found!");
        }

        [Route("solution")]
        [HttpDelete]
        public HttpResponseMessage Delete()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }
    }
}
