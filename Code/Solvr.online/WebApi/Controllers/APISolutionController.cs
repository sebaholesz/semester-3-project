using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("apiV1/")]
    public class APISolutionController : ControllerBase
    {
        private readonly SolutionBusiness solutionBusiness;

        public APISolutionController()
        {
            solutionBusiness = SolutionBusiness.GetSolutionBusiness();
        }

        [Route("solution")]
        [HttpGet]
        public IActionResult Get()
        {
            //Get the List<Assignments>
            List<Solution> solutions = solutionBusiness.GetAllSolutions();

            //Check if the List<Assignments> is not empty and return 200 + solutions if true else return 404 + messasge
            if (solutions.Count > 0)
            {
                return Ok(solutions);
            }
            else
            {
                return NotFound("No Solutions found!");
            }
        }

        [Route("solution/{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment

            Solution solution = solutionBusiness.GetBySolutionId(id);
            if (solution != null)
            {
                return Ok(solution);
            }
            else
            {
                return NotFound("Solution with that ID not found!");
            }
        }




        [Route("solution/byAssignmentId/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsByAssignmentId(int assignmentId)
        {
            //the list is oredered by timestamp

            List<Solution> solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId);
            if (solutions.Count > 0)
            {
                return Ok(solutions);
            }
            else
            {
                return NotFound("Solutions with that AssignmentID not found!");
            }
        }
        
        [Route("solution/count-by-assignmentId/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsCountByAssignmentId(int assignmentId)
        {
            try
            {
                int numberOfSolutions = solutionBusiness.GetSolutionsCountByAssignmentId(assignmentId);

                if (numberOfSolutions >= 0)
                {
                    return Ok(numberOfSolutions);
                }
                else
                {
                    return NotFound("Solutions with that AssignmentID not found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        
        

        [Route("solution")]
        [HttpPost]
        public IActionResult Post([FromBody] Solution solution)
        {
            //Attempt the creation of the solutioin and save the int value of the rows affected
            int queuePosition = solutionBusiness.CreateSolution(solution);

            //Check if the creation was successful and return 201 + queue position or 409 + -1 as a fail message
            if (queuePosition >= 0)
            {
                return StatusCode(201, queuePosition);
            }
            else
            {
                return StatusCode(409, queuePosition);
            }
        }

        [Route("solution/{id}")]
        [HttpPost]
        public IActionResult Post(int id)
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }

        [Route("solution/{id}")]
        [HttpPut]
        public IActionResult Put([FromBody] Solution solution, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = solutionBusiness.UpdateSolution(solution, id);
            if (noOfRows > 0)
            {
                return Ok("Solution Updated Successfuly!");
            }
            else
            {
                return NotFound("Solution was not found!");
            }
        }

        [Route("solution")]
        [HttpPut]
        public IActionResult Put()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }

        [Route("solution/{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
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

        [Route("solution")]
        [HttpDelete]
        public IActionResult Delete()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
        }
    }
}
