using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.IO;

namespace webApi.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly AssignmentBusiness assignmentBusiness;

        public AssignmentController()
        {
            assignmentBusiness = new AssignmentBusiness();
        }

        [Route("assignment/create-assignment")]
        [HttpGet]
        public ActionResult CreateAssignment()
        {
            try
            {
                ViewBag.AcademicLevels = assignmentBusiness.GetAllAcademicLevels();
                ViewBag.Subjects = assignmentBusiness.GetAllSubjects();
                //throw new Exception("The lkdncalkndc failed");
                return View("CreateAssignment");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }

        [Route("assignment/create-assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> CreateAssignmentAsync(IFormCollection collection, IFormFile files)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    Assignment assignment;
                    if (files != null)
                    {
                        var dataStream = new MemoryStream();
                        await files.CopyToAsync(dataStream);

                        assignment = new Assignment(
                        collection["Title"],
                        collection["Description"],
                        Convert.ToInt32(collection["Price"]),
                        Convert.ToDateTime(collection["Deadline"]),
                        Convert.ToBoolean(collection["Anonymous"][0]),
                        collection["AcademicLevel"],
                        collection["Subject"],
                        dataStream.ToArray()
                        );

                        dataStream.Close();
                    }
                    else
                    {
                        assignment = new Assignment(
                           collection["Title"],
                           collection["Description"],
                           Convert.ToInt32(collection["Price"]),
                           Convert.ToDateTime(collection["Deadline"]),
                           Convert.ToBoolean(collection["Anonymous"][0]),
                           collection["AcademicLevel"],
                           collection["Subject"]
                       );
                    }



                    //we will get the id once using the CreateAssignmentWithFile method
                    int assignmentId = assignmentBusiness.CreateAssignmentWithFile(assignment);

                    if (assignmentId >= 0)
                    {
                        ViewBag.Message = "Assignment created successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                        ViewBag.ButtonText = "Display your assignment";
                        ViewBag.ButtonLink = "/assignment/display-assignment/" + assignmentId;
                        ViewBag.PageTitle = "Assignment created!";
                        ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                        ViewBag.Image = "/assets/icons/success.svg";
                    }
                    else
                    {
                        ViewBag.Message = "Assignment creation failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to the assignment form";
                        ViewBag.ButtonLink = "/assignment/create-assignment/";
                        ViewBag.PageTitle = "Assignment creation failed!";
                        ViewBag.SubMessage = "There was a server error \ntry again later";
                        ViewBag.Image = "/assets/icons/error.svg";
                    }
                }
                else
                {
                    ViewBag.Message = "Assignment creation failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to the assignment form";
                    ViewBag.ButtonLink = "/assignment/create-assignment/";
                    ViewBag.PageTitle = "Assignment creation failed!";
                    ViewBag.SubMessage = "Invalid data inserted";
                    ViewBag.Image = "/assets/icons/error.svg";
                }
                return View("UserFeedback");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }

        [Route("assignment/display-assignment/{id}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int id)
        {
            try
            {
                ViewBag.Assignment = assignmentBusiness.GetByAssignmentId(id);
                return View("DisplayAssignment");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }
    }
}
