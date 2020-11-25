using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Collections.Generic;

namespace webApi.Controllers
{
    [Route("api/solution/")]
    [ApiController]
    public class SolutionController : ControllerBase
    {
        private readonly SolutionBusiness solutionBusiness;

        public SolutionController(SolutionBusiness solutionBusiness)
        {
            this.solutionBusiness = solutionBusiness;
        }

        [HttpGet]
        public ActionResult Get()
        {
            //Get the List<Assignments>
            List<Solution> solutions = solutionBusiness.GetAllSolutions();

            //Check if the List<Solution> is not empty and return 200 + solutions if true else return 404 + messasge
            if (solutions.Count > 0)
            {
                //Return 200 + solutions
                return Ok(solutions);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Solutions Found!");
            }
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult Get(int id)
        {
            //return in HttpResonseMessage body Assignment

            Solution solution = solutionBusiness.GetBySolutionId(id);
            if (solution != null)
            {
                //Return 200 + assignments
                return Ok(solution);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Solution Found!");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Solution solution)
        {
            //Attempt the creation of the solutioin and save the int value of the rows affected
            int queuePosition = solutionBusiness.CreateSolution(solution);

            //Check if the creation was successful and return 201 + queue position or 409 + -1 as a fail message
            if (queuePosition >= 0)
            {
                //Return 201 + string with message
                return Ok(queuePosition);
            }
            else
            {
                //Return 409 + string with message
                return NotFound("Solution Creation Failed");
            }
        }

        [Route("{id}")]
        [HttpPost]
        public ActionResult Post()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Put([FromBody] Solution solution, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = solutionBusiness.UpdateSolution(solution, id);
            if (noOfRows > 0)
            {
                return Ok(noOfRows);
            }
            else
            {
                return NotFound("Solution was not found!");
            }
        }

        [HttpPut]
        public ActionResult Put()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            //return if operation was successful

            int noOfRows = solutionBusiness.DeleteSolution(id);
            if (noOfRows > 0)
            {
                return Ok("Solution Deleted Successfuly!");
            }
            else
            {
                return NotFound("Solution was not found!");
            }
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }
    }
}
