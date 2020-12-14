﻿using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WebApi.Controllers
{
    [Authorize()]
    [ApiController]
    [Route("apiV1/")]
    public class APIAssignmentController : ControllerBase
    {
        [Route("assignment")]
        [HttpPost]
        public IActionResult CreateAssignment([FromBody] Assignment assignment)
        {
            try
            {
                assignment.PostDate = DateTime.UtcNow;
                int insertedAssignmentId = AssignmentBusiness.GetAssignmentBusiness().CreateAssignment(assignment);
                //decrease creditu
                bool successfulyDecreased = UserBusiness.GetUserBusiness().DecreaseUserCredits(assignment.Price, assignment.UserId) == 1;
                if (insertedAssignmentId > 0 && successfulyDecreased)
                {
                    return Ok(insertedAssignmentId);
                }
                else
                {
                    return StatusCode(417);
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("assignment")]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);
                


                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignments();
                    if (assignments.Count() > 0)
                    {
                        return Ok(assignments);
                    }
                   
                }
                return NotFound();
                
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("assignment/complete-data/{assignmentId}")]
        [HttpGet]   
        public IActionResult GetCompleteData(int assignmentId)
        {
            try
            {
                object assignmentCompleteData = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteData(assignmentId);

                if (assignmentCompleteData != null)
                {
                    return Ok(assignmentCompleteData);
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

        /*ONLY SOLVER*/
        [Route("assignment/complete-data-with-solution/{assignmentId}")]
        [HttpGet]
        public IActionResult GetCompleteDataWithSolution(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isSolver = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 2;
                if (isSolver)
                {
                    object assignmentCompleteDataWithSolution = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteDataWithSolution(assignmentId, userId);

                    if (assignmentCompleteDataWithSolution != null)
                    {
                        return Ok(assignmentCompleteDataWithSolution);
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

        /*ONLY AUTHOR OR ACCEPTED SOLUTION AUTHOR*/
        [Route("assignment/complete-data-with-accepted-solution/{assignmentId}")]
        [HttpGet]
        public IActionResult GetCompleteDataWithAcceptedSolution(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                bool isSolutionAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 2;
                if (isAuthor)
                {
                    object assignmentCompleteDataWithAcceptedSolution = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteDataWithAcceptedSolution(assignmentId);

                    if (assignmentCompleteDataWithAcceptedSolution != null)
                    {
                        return Ok(assignmentCompleteDataWithAcceptedSolution);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                if (isSolutionAuthor)
                {
                    dynamic assignmentCompleteDataWithAcceptedSolution = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteDataWithAcceptedSolution(assignmentId);

                    if (assignmentCompleteDataWithAcceptedSolution != null)
                    {
                        if (assignmentCompleteDataWithAcceptedSolution.Solution.UserId == userId)
                        {
                            return Ok(assignmentCompleteDataWithAcceptedSolution);
                        }
                        else
                        {
                            return Unauthorized();
                        }
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

        [AllowAnonymous]
        [Route("assignment/all-active")]
        [HttpGet]
        public IActionResult GetAllActiveAssignments()
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignments();

                if (assignments.Count() > 0)
                {
                    return Ok(assignments);
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

        [AllowAnonymous]
        [Route("assignment/page-all-active/{pageNumber}")]
        [HttpGet]
        public IActionResult GetActiveAssignmentsByPage(int pageNumber)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetActiveAssignmentsByPage(pageNumber);

                if (assignments.Count() > 0)
                {
                    int count = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentsCount();
                    int totalPages = (int)Math.Ceiling(count / 12.00);
                    bool previousPage = pageNumber > 1 ? true : false;
                    bool nextPage = pageNumber < totalPages ? true : false;
                    var paginationMetadata = new
                    {
                        count,
                        totalPages,
                        previousPage,
                        nextPage

                    };
                    HttpContext.Response.Headers.Add("PagingHeaders", JsonConvert.SerializeObject(paginationMetadata)); 
                    return Ok(assignments);
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

        [Route("assignment/all-active-not-posted-by-user")]
        [HttpGet]
        public IActionResult GetAllActiveAssignmentsNotPostedByUser()
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotPostedByUser(userId);

                if (assignments.Count() > 0)
                {
                    return Ok(assignments);
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

        [Route("assignment/page-all-active-not-posted-by-user/{pageNumber}")]
        [HttpGet]
        public IActionResult GetAllActiveAssignmentsNotPostedByUserPage(int pageNumber)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotPostedByUserPage(userId, pageNumber);

                if (assignments.Count() > 0)
                {
                    int count = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentsCountNotByUser(userId);
                    int totalPages = (int)Math.Ceiling(count / 12.00);
                    bool previousPage = pageNumber > 1 ? true : false;
                    bool nextPage = pageNumber < totalPages ? true : false;
                    var paginationMetadata = new
                    {
                        count,
                        totalPages,
                        previousPage,
                        nextPage

                    };
                    HttpContext.Response.Headers.Add("PagingHeaders", JsonConvert.SerializeObject(paginationMetadata));
                    return Ok(assignments);
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

        [Route("assignment/user/{username}")]
        [HttpGet]
        public IActionResult GetAllAssignmentsForUser(string username)
        {
            try
            {
                string userId = UserBusiness.GetUserBusiness().GetUserByUserName(username).Id;
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsForUser(userId);

                if (assignments.Count() > 0)
                {
                    return Ok(assignments);
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

        [Route("assignment/page-user/{pageNumber}")]
        [HttpGet]
        public IActionResult GetAllAssignmentsForUserPage(int pageNumber)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsForUserPage(userId, pageNumber);

                if (assignments.Count() > 0)
                {
                    int count = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentsCountForUser(userId);
                    int totalPages = (int)Math.Ceiling(count / 12.00);
                    bool previousPage = pageNumber > 1 ? true : false;
                    bool nextPage = pageNumber < totalPages ? true : false;
                    var paginationMetadata = new
                    {
                        count,
                        totalPages,
                        previousPage,
                        nextPage

                    };
                    HttpContext.Response.Headers.Add("PagingHeaders", JsonConvert.SerializeObject(paginationMetadata));
                    return Ok(assignments);
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

        /*ONLY SOLVER = CAN ONLY ACCESS HIS INFO*/
        [Route("assignment/solved-by-user")]
        [HttpGet]
        public IActionResult GetAllAssignmentsSolvedByUser()
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);

                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsSolvedByUser(userId);

                if (assignments.Count() > 0)
                {
                    return Ok(assignments);
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

        /*ONLY BY THE "NOT SOLVER" = CAN ONLY ACCESS HIS INFO*/
        [Route("assignment/get-active-not-solved-by-user")]
        [HttpGet]
        public IActionResult GetAllActiveAssignmentsNotSolvedByUser()
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);

                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotSolvedByUser(userId);

                if (assignments.Count() > 0)
                {
                    return Ok(assignments);
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

        [Route("assignment/{id}")]
        [HttpGet]
        public IActionResult GetByAssignmentId(int id)
        {
            try
            {
                Assignment assignment = AssignmentBusiness.GetAssignmentBusiness().GetByAssignmentId(id);
                if (!assignment.Equals(null))
                {
                    return Ok(assignment);
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

        /*ONLY BY AUTHOR*/
        [Route("assignment/{assignmentId}")]
        [HttpPut]
        public IActionResult Put([FromBody] Assignment assignment, int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
                {
                    int noOfRowsAffected = AssignmentBusiness.GetAssignmentBusiness().UpdateAssignment(assignment, assignmentId);
                    if (noOfRowsAffected > 0)
                    {
                        return Ok("Assignment updated successfully!");
                    }
                    else
                    {
                        return NotFound($"Assignment with id {assignmentId} was not found");
                    }
                }
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /*ONLY BY AUTHOR*/
        [Route("assignment/check-if-has-accepted-solution/{assignmentId}")]
        [HttpGet]
        public IActionResult CheckIfHasAcceptedSolution(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
                {
                    bool hasAcceptedSolution = AssignmentBusiness.GetAssignmentBusiness().CheckIfHasAcceptedSolution(assignmentId);
                    if (hasAcceptedSolution)
                    {
                        return Ok();
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

        /*ONLY BY AUTHOR*/
        [Route("assignment/inactive/{assignmentId}")]
        [HttpPut]
        public IActionResult MakeInactive(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                bool isAuthor = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId) == 1;
                if (isAuthor)
                {
                    int noOfRowsAffected = AssignmentBusiness.GetAssignmentBusiness().MakeInactive(assignmentId);
                    if (noOfRowsAffected > 0)
                    {
                        return Ok();
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

      
        [Route("assignment/active/{id}")]
        [HttpPut]
        public IActionResult MakActive(int id)
        {
            try
            {
                string userName = APIAuthenticationController.GetUserNameFromRequestHeader(Request.Headers);
                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(userName))
                {
                    int noOfRowsAffected = AssignmentBusiness.GetAssignmentBusiness().MakeActive(id);
                    if (noOfRowsAffected > 0)
                    {
                        return Ok();
                    }
                }
                return NotFound("Neither admin nor moderator!");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }            
        }

        [Route("academiclevel")]
        [HttpGet]
        public IActionResult GetAllAcademicLevels()
        {
            try
            {
                List<string> levels = AssignmentBusiness.GetAssignmentBusiness().GetAllAcademicLevels();

                if (levels.Count() > 0)
                {
                    return Ok(levels);
                }
                else
                {
                    return NotFound("No Academic levels Found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("subject")]
        [HttpGet]
        public IActionResult GetAllSubjects()
        {
            try
            {
                List<string> subjects = AssignmentBusiness.GetAssignmentBusiness().GetAllSubjects();

                if (subjects.Count() > 0)
                {
                    return Ok(subjects);
                }
                else
                {
                    return NotFound("No Subjects Found!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("check-user-vs-assignment/{assignmentId}")]
        [HttpGet]
        public IActionResult CheckUserVsAssignment(int assignmentId)
        {
            try
            {
                string userId = APIAuthenticationController.GetUserIdFromRequestHeader(Request.Headers);
                int returnCode = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, userId);

                if (new[] { 0, 1, 2 }.Contains(returnCode))
                {
                    return Ok(returnCode);
                }
                else
                {
                    return NotFound("This user is not associated with the assignment!");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("assignment/get-file/{assignmentId}")]
        [HttpGet]
        public IActionResult GetFileFromDB(int assignmentId)
        {
            try
            {
                byte[] fileContent = AssignmentBusiness.GetAssignmentBusiness().GetFileFromDB(assignmentId);
                if(fileContent.Length > 1)
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