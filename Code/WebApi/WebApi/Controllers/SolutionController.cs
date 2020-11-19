using BusinessLayer;
using ModelLayer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class SolutionController : ApiController
    {
        private readonly SolutionBusiness solutionBusiness;

        public SolutionController()
        {
            solutionBusiness = new SolutionBusiness();
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get the List<Assignments>
            List<Solution> solutions = solutionBusiness.GetAllSolutions();

            //Check if the List<Assignments> is not empty and return 200 + solutions if true else return 404 + messasge
            return solutions.Count > 0 ? Request.CreateResponse(HttpStatusCode.OK, solutions) : Request.CreateResponse(HttpStatusCode.NotFound, "No Solutions found!");
        }

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment

            Solution solution = solutionBusiness.GetBySolutionId(id);
            return solution != null ? Request.CreateResponse(HttpStatusCode.OK, solution) : Request.CreateResponse(HttpStatusCode.NotFound, "Solution with that ID not found!");
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Solution solution)
        {
            //Attempt the creation of the solutioin and save the int value of the rows affected
            //int rowsAffected = solutionBusiness.CreateSolution(solution);

            int queuePosition = solutionBusiness.CreateSolution(solution);
            //Check if the creation was successful and return 201 + string message or 409 with string message if false
            //return rowsAffected > 0 ? Request.CreateResponse(HttpStatusCode.Created, "Solution Created Successfuly!") : Request.CreateResponse(HttpStatusCode.NotFound, "Solution Creation Failed");
            return queuePosition >= 0 ? Request.CreateResponse(HttpStatusCode.Created, queuePosition) : Request.CreateResponse(HttpStatusCode.NotFound, queuePosition);
        }

        [HttpPost]
        public HttpResponseMessage Post(int id)
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] Solution solution, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = solutionBusiness.UpdateSolution(solution, id);
            return noOfRows > 0 ? Request.CreateResponse(HttpStatusCode.OK, "Solution Updated Successfuly!") : Request.CreateResponse(HttpStatusCode.NotFound, "Solution was not found!");
        }

        [HttpPut]
        public HttpResponseMessage Put()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            //return if operation was successful

            int noOfRows = solutionBusiness.DeleteSolution(id);
            return noOfRows > 0 ? Request.CreateResponse(HttpStatusCode.OK, "Solution Deleted Successfuly!") : Request.CreateResponse(HttpStatusCode.NotFound, "Solution was not found!");
        }

        [HttpDelete]
        public HttpResponseMessage Delete()
        {
            //Invalid request which returns 400
            return Request.CreateResponse(HttpStatusCode.BadRequest, "The URL Is Invalid - Bad Request!");
        }
    }
}
