using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("apiV1/")]
    public class APISolutionController : ControllerBase
    {
        [Route("solution")]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Solution> solutions = SolutionBusiness.GetSolutionBusiness().GetAllSolutions();

                if (solutions.Count > 0)
                {
                    return Ok(solutions);
                }
                else
                {
                    return NotFound("No Solutions found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("solution/{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                Solution solution = SolutionBusiness.GetSolutionBusiness().GetBySolutionId(id);
                if (solution != null)
                {
                    return Ok(solution);
                }
                else
                {
                    return NotFound("Solution with that ID not found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("solution/by-assignment/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsByAssignmentId(int assignmentId)
        {
            try
            {
                List<Solution> solutions = SolutionBusiness.GetSolutionBusiness().GetSolutionsByAssignmentId(assignmentId);
                
                if (solutions.Count > 0)
                {
                    return Ok(solutions);
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
        
        [Route("solution/count-by-assignmentId/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsCountByAssignmentId(int assignmentId)
        {
            try
            {
                int numberOfSolutions = SolutionBusiness.GetSolutionBusiness().GetSolutionsCountByAssignmentId(assignmentId);

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

        [Route("solution/choose-solution")]
        [HttpPost]
        public IActionResult ChooseSolution([FromBody] List<int> ids )
        {
            try
            {
                int solutionId = ids[0];
                int assignmentId = ids[1];
                bool response = SolutionBusiness.GetSolutionBusiness().ChooseSolution(solutionId, assignmentId);
                if(response)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(409);
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
            try
            {
                int queuePosition = SolutionBusiness.GetSolutionBusiness().CreateSolution(solution);

                if (queuePosition > 0)
                {
                    return StatusCode(201, queuePosition);
                }
                else
                {
                    return StatusCode(409);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("solution/{id}")]
        [HttpPut]
        public IActionResult Put([FromBody] Solution solution, int id)
        {
            try
            {
                int noOfRowsAffected = SolutionBusiness.GetSolutionBusiness().UpdateSolution(solution, id);
                if (noOfRowsAffected == 1)
                {
                    return Ok("Solution Updated Successfuly!");
                }
                else
                {
                    return NotFound("Solution was not found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("solution/{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                int noOfRowsAffected = SolutionBusiness.GetSolutionBusiness().DeleteSolution(id);
                if (noOfRowsAffected == 1)
                {
                    return Ok("Solution Deleted Successfuly!");
                }
                else
                {
                    return NotFound("Solution was not found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("solution/get-file/{solutionId}")]
        [HttpPost]
        public IActionResult GetFileFromDB([FromBody] User user, int solutionId)
        {
            try
            {
                byte[] fileContent = SolutionBusiness.GetSolutionBusiness().GetFileFromDB(solutionId, user);
                if (fileContent.Length > 1)
                {
                    return Ok(fileContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
