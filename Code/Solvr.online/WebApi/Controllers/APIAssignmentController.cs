using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("apiV1/")]
    public class APIAssignmentController : ControllerBase
    {
        private readonly AssignmentBusiness assignmentBusiness;

        public APIAssignmentController()
        {
            assignmentBusiness = AssignmentBusiness.GetAssignmentBusiness();
        }

        [Route("assignment")]
        [HttpPost]
        public IActionResult CreateAssignment([FromBody] Assignment assignment)
        {
            try
            {
                assignment.PostDate = DateTime.Now;
                int insertedAssignmentId = assignmentBusiness.CreateAssignment(assignment);

                //200 OK = if everything went well
                //417 Expected value is not the same as the actual value = if noOfRowsAffected is not 1
                //400 Bad Request = if invalid data was used for the post request
                //500 Server Error = if an exception was thrown
                switch (insertedAssignmentId)
                {
                    case int iaid when (iaid > 0): return Ok(insertedAssignmentId);
                    case 0  : return StatusCode(417);
                    case -1 : return BadRequest("Invalid data inserted");
                    default: throw new HttpRequestException();
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
            List<Assignment> assignments = assignmentBusiness.GetAllAssignments();

            if (assignments.Count() > 0)
            {
                return Ok(assignments);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("assignment/complete-data/{assignmentId}")]
        [HttpGet]
        public IActionResult GetCompleteData(int assignmentId)
        {
            try
            {
                object assignmentCompleteData = assignmentBusiness.GetAssignmentCompleteData(assignmentId);

                if (assignmentCompleteData != null )
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

        [Route("assignment/complete-data-with-solution/{assignmentId}")]
        [HttpGet]
        public IActionResult GetCompleteDataWithSolution(int assignmentId)
        {
            try
            {
                object assignmentCompleteDataWithSolution = assignmentBusiness.GetAssignmentCompleteDataWithSolution(assignmentId);

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

        [Route("assignment/all-active")]
        [HttpGet]
        public IActionResult GetAllActiveAssignments()
        {
            try
            {
                List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignments();

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

        [Route("assignment/all-active-not-posted-by-user")]
        [HttpPost]
        public IActionResult GetAllActiveAssignmentsNotPostedByUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignmentsNotPostedByUser(user.Id);

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

        [Route("assignment/user")]
        [HttpPost]
        public IActionResult GetAllAssignmentsForUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = assignmentBusiness.GetAllAssignmentsForUser(user.Id);

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

        [Route("assignment/solved-by-user")]
        [HttpGet]
        public IActionResult GetAllAssignmentsSolvedByUser([FromBody] User user)
        {
            try
            {
                List<Assignment> assignments = assignmentBusiness.GetAllAssignmentsSolvedByUser(user.Id);

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
            List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignmentsNotSolvedByUser(user.Id);

            if (assignments.Count() > 0)
            {
                return Ok(assignments);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("assignment/{id}")]
        [HttpGet]
        public Assignment GetByAssignmentId(int id)
        {
            Assignment assignment = assignmentBusiness.GetByAssignmentId(id);
            return assignment ?? null;
        }

        [Route("assignment/{id}")]
        [HttpPut]
        public IActionResult Put([FromBody] Assignment assignment, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = assignmentBusiness.UpdateAssignment(assignment, id);
            if (noOfRows > 0)
            {
                return Ok("Assignment updated successfully!");
            }
            else
            {
                return NotFound($"Assignment with id {id} was not found");
            }
        }

        [Route("assignment/inactive/{id}")]
        [HttpPut]
        public IActionResult MakeInactive(int id)
        {
            int noOfRows = assignmentBusiness.MakeInactive(id);
            if (noOfRows > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [Route("assignment/active/{id}")]
        [HttpPut]
        public IActionResult MakActive(int id)
        {
            int noOfRows = assignmentBusiness.MakeActive(id);
            if (noOfRows > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [Route("academiclevel")]
        [HttpGet]
        public IActionResult GetAllAcademicLevels()
        {
            //Get the List<AcademicLevel>
            List<string> levels = assignmentBusiness.GetAllAcademicLevels();

            //Check if the List<AcademicLevel> is not empty
            if (levels.Count() > 0)
            {
                //Return 200 + levels
                return Ok(levels);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Academic levels Found!");
            }
        }

        [Route("subject")]
        [HttpGet]
        public IActionResult GetAllSubjects()
        {
            //Get the List<Subject>
            List<string> subjects = assignmentBusiness.GetAllSubjects();

            //Check if the List<Subject> is not empty
            if (subjects.Count() > 0)
            {
                //Return 200 + subjects
                return Ok(subjects);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Subjects Found!");
            }
        }

        [Route("check-user-vs-assignment/{assignmentId}")]
        [HttpPost]
        public IActionResult CheckUserVsAssignment([FromBody] User user, int assignmentId)
        {
            try
            {
                //Get the List<Subject>
                int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, user.Id);

                //Check if the List<Subject> is not empty
                if (new[] { 0, 1, 2 }.Contains(returnCode))
                {
                    //Return 200 + subjects
                    return Ok(returnCode);
                }
                else
                {
                    return NotFound("This user is not associated with the assignment!");
                }
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}