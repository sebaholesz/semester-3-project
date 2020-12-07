using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Collections.Generic;
using System.Linq;

namespace webApi.Controllers
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
        [HttpGet]
        public IActionResult Get()
        {
            //Get the List<Assignments>
            List<Assignment> assignments = assignmentBusiness.GetAllAssignments();

            //Check if the List<Assignments> is not empty
            if (assignments.Count() > 0)
            {
                return Ok(assignments);
            }
            else
            {
                return NotFound();
            }
        }


        ////[Route("assignment/{id}")]
        //[HttpGet]
        //public Assignment GetByAssignmentId(int id)
        //{
        //    Assignment assignment = assignmentBusiness.GetByAssignmentId(id);
        //    return assignment != null ? assignment : null;
        //}


        [Route("assignment")]
        [HttpPost]
        public int CreateAssignment([FromBody] Assignment assignment)
        {
            int rowsAffected = assignmentBusiness.CreateAssignment(assignment);

            //Check if the creation was successful
            if (rowsAffected > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }


        //[Route("assignment/{id}")]
        //[HttpPost]
        //public HttpResponseMessage PostCreateAssignmentWithId(int id)
        //{
        //    //Invalid request which returns 400
        //    return 
        //}


        [Route("assignment")]
        [HttpPut]
        public IActionResult Put()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
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


        [Route("assignment/inactive")]
        [HttpPut]
        public IActionResult MakeInactive()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
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

        [Route("assignment/active")]
        [HttpPut]
        public IActionResult MakeActive()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request!");
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
    }
}