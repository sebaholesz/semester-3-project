using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections;
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
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);
               
                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    List<Solution> solutions = SolutionBusiness.GetSolutionBusiness().GetAllSolutions();
                    if (solutions.Count > 0)
                    {
                        return Ok(solutions);
                    }
                }
                return NotFound("No Solutions found!");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        
        /*ONLY NOT-SOLVERS AND NOT-AUTHORS*/
        [Route("solution")]
        [HttpPost]
        public IActionResult Post([FromBody] Solution solution)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool hasNoConnectionToAssignment = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(solution.AssignmentId, userId) == 0;

                if (hasNoConnectionToAssignment)
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
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
       
        #region unused GetSolutionById

        //[Route("solution/{id}")]
        //[HttpGet]
        //public IActionResult Get(int id)
        //{
        //    try
        //    {
        //        Solution solution = SolutionBusiness.GetSolutionBusiness().GetBySolutionId(id);
        //        if (solution != null)
        //        {
        //            return Ok(solution);
        //        }
        //        else
        //        {
        //            return NotFound("Solution with that ID not found!");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500);
        //    }
        //}

        #endregion
        
        /*ONLY AUTHOR*/
        [Route("solution/by-assignment/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
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
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY AUTHOR*/
        [Route("solution/count-by-assignmentId/{assignmentId}")]
        [HttpGet]
        public IActionResult GetSolutionsCountByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
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
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY AUTHOR*/
        [Route("solution/choose-solution")]
        [HttpPost]
        public IActionResult ChooseSolution([FromBody] List<int> ids)
        {
            try
            {
                int solutionId = ids[0];
                int assignmentId = ids[1];

                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
                {
                    bool response = SolutionBusiness.GetSolutionBusiness().ChooseSolution(solutionId, assignmentId);
                    if (response)
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(409);
                    }
                }
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY SOLVER*/
        [Route("solution/{solutionid}")]
        [HttpPut]
        public IActionResult Put([FromBody] Solution solution, int solutionid)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isSolutionAuthor = SolutionBusiness.GetSolutionBusiness().CheckIfUserIsSolutionAuthor(userId, solutionid);
                if (isSolutionAuthor)
                {
                    int noOfRowsAffected = SolutionBusiness.GetSolutionBusiness().UpdateSolution(solution, solutionid);
                    if (noOfRowsAffected == 1)
                    {
                        return Ok("Solution Updated Successfuly!");
                    }
                    else
                    {
                        return NotFound("Solution was not found!");
                    }
                }
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY SOLVER*/
        [Route("solution/{solutionid}")]
        [HttpDelete]
        public IActionResult Delete(int solutionid)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isSolutionAuthor = SolutionBusiness.GetSolutionBusiness().CheckIfUserIsSolutionAuthor(userId, solutionid);
                if (isSolutionAuthor)
                {
                    int noOfRowsAffected = SolutionBusiness.GetSolutionBusiness().DeleteSolution(solutionid);
                    if (noOfRowsAffected == 1)
                    {
                        return Ok("Solution Deleted Successfuly!");
                    }
                    else
                    {
                        return NotFound("Solution was not found!");
                    }
                }
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY SOLVER OR AUTHOR AFTER ACCEPTATION*/
        [Route("solution/get-file/{solutionId}")]
        [HttpPost]
        public IActionResult GetFileFromDB(int solutionId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                if (SolutionBusiness.GetSolutionBusiness().CheckIfUserCanDownloadSolutionFile(solutionId, userId))
                {
                    byte[] fileContent = SolutionBusiness.GetSolutionBusiness().GetFileFromDB(solutionId);
                    if (fileContent.Length > 1)
                    {
                        return Ok(fileContent);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
