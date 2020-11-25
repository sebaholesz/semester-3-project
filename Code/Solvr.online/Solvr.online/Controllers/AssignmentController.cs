using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace webApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly AssignmentBusiness assignmentBusiness;
        //private readonly IDbAssignment dbAssignment;

        public AssignmentController(AssignmentBusiness assignmentBusiness)
        {
            this.assignmentBusiness = assignmentBusiness;
        }

        [Route("assignment")]
        [HttpGet]
        public ActionResult Get()
        {
            //Get the List<Assignments>
            List<Assignment> assignments = assignmentBusiness.GetAllAssignments();

            //Check if the List<Assignments> is not empty
            if (assignments.Count() > 0)
            {
                //Return 200 + assignments
                return Ok(assignments);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Assigments Found!");
            }
        }


        [Route("assignment/{id}")]
        [HttpGet]
        public ActionResult Get(int id)
        {
            //assignmentInterface get
            //return in HttpResonseMessage body Assignment

            var assignment = assignmentBusiness.GetByAssignmentId(id);
            if (assignment != null)
            {
                //Return 200 + assignments
                return Ok(assignment);
            }
            else
            {
                //Return 404 + string with message
                return NotFound("No Assigments Found!");
            }
        }


        [Route("assignment")]
        [HttpPost]
        public ActionResult Post([FromBody] Assignment assignment)
        {
            //Attempt the creation of the assignment and save the bool value of the result
            //bool wasSuccesful = assignmentBusiness.CreateAssignment(assignment);
            int rowsAffected = assignmentBusiness.CreateAssignment(assignment);

            //Check if the creation was successful
            if (rowsAffected > 0)
            {
                //Return 201 + string with message
                return Ok("Assignment Created Successfuly!");
            }
            else
            {
                //Return 409 + string with message
                return NotFound("Assignment Creation Failed");
            }
        }


        [Route("assignment/{id}")]
        [HttpPost]
        public ActionResult Post()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request");
        }


        [Route("assignment")]
        [HttpPut]
        public ActionResult Put()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request");
        }


        [Route("assignment/{id}")]
        [HttpPut]
        public HttpResponseMessage Put([FromBody] Assignment assignment, int id)
        {
            //assignmentInterface update
            //return in HttpResonseMessage body Assignment

            int noOfRows = assignmentBusiness.UpdateAssignment(assignment, id);
            return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        [Route("assignment")]
        [HttpDelete]
        public ActionResult Delete()
        {
            //Invalid request which returns 400
            return BadRequest("The URL Is Invalid - Bad Request");
        }


        [Route("assignment/{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            //assignmentInterface delete
            //return if operation was successful

            int noOfRows = assignmentBusiness.DeleteAssignment(id);
            return noOfRows > 0 ? new HttpResponseMessage(HttpStatusCode.OK) : new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [Route("academiclevel")]
        [HttpGet]
        public ActionResult GetAllAcademicLevels()
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
        public ActionResult GetAllSubjects()
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
