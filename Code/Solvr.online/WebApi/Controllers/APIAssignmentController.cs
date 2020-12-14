using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

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
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignments();

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

        //[Route("assignment/complete-data-with-solution/{assignmentId}")]
        //[HttpGet]
        //public IActionResult GetCompleteDataWithSolution(int assignmentId)
        //{
        //    try
        //    {
        //        object assignmentCompleteDataWithSolution = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteDataWithSolution(assignmentId);

        //        if (assignmentCompleteDataWithSolution != null)
        //        {
        //            return Ok(assignmentCompleteDataWithSolution);
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500);
        //    }
        //}

        [Route("assignment/complete-data-with-accepted-solution/{assignmentId}")]
        [HttpGet]
        public IActionResult GetCompleteDataWithAcceptedSolution(int assignmentId)
        {
            try
            {
                object assignmentCompleteDataWithSolution = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentCompleteDataWithAcceptedSolution(assignmentId);

                if (assignmentCompleteDataWithSolution != null)
                {
                    return Ok(assignmentCompleteDataWithSolution);
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
        [HttpPost]
        public IActionResult GetAllActiveAssignmentsNotPostedByUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotPostedByUser(user.Id);

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
        [HttpPost]
        public IActionResult GetAllActiveAssignmentsNotPostedByUserPage([FromBody] User user, int pageNumber)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotPostedByUserPage(user.Id, pageNumber);

                if (assignments.Count() > 0)
                {
                    int count = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentsCountNotByUser(user.Id);
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

        [Route("assignment/user")]
        [HttpPost]
        public IActionResult GetAllAssignmentsForUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsForUser(user.Id);

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
        [HttpPost]
        public IActionResult GetAllAssignmentsForUserPage([FromBody] User user, int pageNumber)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsForUserPage(user.Id, pageNumber);

                if (assignments.Count() > 0)
                {
                    int count = AssignmentBusiness.GetAssignmentBusiness().GetAssignmentsCountForUser(user.Id);
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

        [Route("assignment/solved-by-user")]
        [HttpPost]
        public IActionResult GetAllAssignmentsSolvedByUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllAssignmentsSolvedByUser(user.Id);

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

        [Route("assignment/get-active-not-solved-by-user")]
        [HttpGet]
        public IActionResult GetAllActiveAssignmentsNotSolvedByUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = AssignmentBusiness.GetAssignmentBusiness().GetAllActiveAssignmentsNotSolvedByUser(user.Id);

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

        [Route("assignment/{id}")]
        [HttpPut]
        public IActionResult Put([FromBody] Assignment assignment, int id)
        {
            try
            {
                int noOfRowsAffected = AssignmentBusiness.GetAssignmentBusiness().UpdateAssignment(assignment, id);
                if (noOfRowsAffected > 0)
                {
                    return Ok("Assignment updated successfully!");
                }
                else
                {
                    return NotFound($"Assignment with id {id} was not found");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Route("assignment/inactive/{id}")]
        [HttpPut]
        public IActionResult MakeInactive(int id)
        {
            try
            {
                int noOfRowsAffected = AssignmentBusiness.GetAssignmentBusiness().MakeInactive(id);
                if (noOfRowsAffected > 0)
                {
                    return Ok();
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

        [Route("assignment/active/{id}")]
        [HttpPut]
        public IActionResult MakActive(int id, [FromBody]User user)
        {
            try
            {
                if (UserBusiness.GetUserBusiness().CheckIfAdminOrModerator(user.UserName))
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
        [HttpPost]
        public IActionResult CheckUserVsAssignment([FromBody] User user, int assignmentId)
        {
            try
            {
                int returnCode = AssignmentBusiness.GetAssignmentBusiness().CheckUserVsAssignment(assignmentId, user.Id);

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