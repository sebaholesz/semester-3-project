using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

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
        public ActionResult CreateAssignment(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Assignment assignment = new Assignment(
                        collection["Title"],
                        collection["Description"],
                        Convert.ToInt32(collection["Price"]),
                        Convert.ToDateTime(collection["Deadline"]),
                        Convert.ToBoolean(collection["Anonymous"][0]),
                        collection["AcademicLevel"],
                        collection["Subject"]
                    );

                    int rowsAffected = assignmentBusiness.CreateAssignment(assignment);

                    if (rowsAffected > 0)
                    {
                        ViewBag.AcademicLevels = assignmentBusiness.GetAllAcademicLevels();
                        ViewBag.Subjects = assignmentBusiness.GetAllSubjects();
                        ViewBag.Message = "Assignment created successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                    }
                    else
                    {
                        ViewBag.AcademicLevels = assignmentBusiness.GetAllAcademicLevels();
                        ViewBag.Subjects = assignmentBusiness.GetAllSubjects();
                        ViewBag.Message = "Assignment creation failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                    }
                    return View("CreateAssignment");
                }
                else
                {
                    ViewBag.Message = "Insert correct data";
                    ViewBag.ResponseStyleClass = "text-danger";
                    return View("CreateAssignment");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                ViewBag.ResponseStyleClass = "text-danger";
                return View("CreateAssignment");
            }
        }

        [Route("assignment/display-assignment/{id}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int id)
        {
            try
            {
                ViewBag.Assignment = assignmentBusiness.GetByAssignmentId(id);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return View("DisplayAssignment");
        }
    }
}
