﻿using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace webApi.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly AssignmentBusiness assignmentBusiness;
        private readonly SolutionBusiness solutionBusiness;

        public AssignmentController()
        {
            assignmentBusiness = new AssignmentBusiness();
            solutionBusiness = new SolutionBusiness();
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
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/create-assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAssignmentAsync(IFormCollection collection, IFormFile files)
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
                            DateTime.Now,
                            Convert.ToDateTime(collection["Deadline"]),
                            Convert.ToBoolean(collection["Anonymous"][0]),
                            collection["AcademicLevel"],
                            collection["Subject"],
                            dataStream.ToArray(),
                            User.FindFirstValue(ClaimTypes.NameIdentifier)
                        );

                        dataStream.Close();
                    }
                    else
                    {
                        assignment = new Assignment(
                           collection["Title"],
                           collection["Description"],
                           Convert.ToInt32(collection["Price"]),
                           DateTime.Now,
                           Convert.ToDateTime(collection["Deadline"]),
                           Convert.ToBoolean(collection["Anonymous"][0]),
                           collection["AcademicLevel"],
                           collection["Subject"],
                           User.FindFirstValue(ClaimTypes.NameIdentifier)
                       );
                    }
                    //we will get the id once using the CreateAssignmentWithFile method
                    int assignmentId = assignmentBusiness.CreateAssignment(assignment);

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
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/display-assignment/{id}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int id)
        {
            try
            {
                ViewBag.Assignment = assignmentBusiness.GetByAssignmentId(id);
                ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(id).Count;
                return View("DisplayAssignment");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [AllowAnonymous]
        [Route("assignment/display-assignments")]
        [HttpGet]
        public ActionResult DisplayAllAssignments()
        {
            try
            {
                List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignments();
                if (assignments.Count > 0)
                {
                    ViewBag.Assignments = assignments;
                    ViewBag.ShowSolvedByUser = false;


                    return View("AllAssignments");
                }
                else
                {
                    ViewBag.Message = "No assignment found";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "No assignments found!";
                    ViewBag.SubMessage = "There were no assignments \nfor the given query";
                    ViewBag.Image = "/assets/icons/error.svg";
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/update-assignment/{id}")]
        [HttpGet]
        public ActionResult UpdateAssignment(int id)
        {
            try
            {
                Assignment assignment = assignmentBusiness.GetByAssignmentId(id);
                ViewBag.Assignment = assignment;
                ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
                return View("UpdateAssignment");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/update-assignment/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAssignment(IFormCollection collection, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //TODO notify all solvers of the changes
                    Assignment assignment = new Assignment(
                        collection["Title"],
                        collection["Description"],
                        Convert.ToInt32(collection["Price"]),
                        DateTime.Now,
                        Convert.ToDateTime(collection["Deadline"]),
                        Convert.ToBoolean(collection["Anonymous"][0]),
                        collection["AcademicLevel"],
                        collection["Subject"],
                        Encoding.ASCII.GetBytes(collection["AssignmentFile"])
                    );

                    int noOfRowsAffected = assignmentBusiness.UpdateAssignment(assignment, id);

                    if (noOfRowsAffected > 0)
                    {
                        ViewBag.Message = "Assignment updated successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                        ViewBag.ButtonText = "Display your assignment";
                        ViewBag.ButtonLink = "/assignment/display-assignment/" + id;
                        ViewBag.PageTitle = "Assignment updated!";
                        ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                        ViewBag.Image = "/assets/icons/success.svg";
                    }
                    else
                    {
                        ViewBag.Message = "Assignment update failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to the assignment form";
                        ViewBag.ButtonLink = "/assignment/update-assignment/" + id;
                        ViewBag.PageTitle = "Assignment update failed!";
                        ViewBag.SubMessage = "There was a server error \ntry again later";
                        ViewBag.Image = "/assets/icons/error.svg";
                    }
                }
                else
                {
                    ViewBag.Message = "Assignment update failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to the assignment form";
                    ViewBag.ButtonLink = "/assignment/update-assignment/" + id;
                    ViewBag.PageTitle = "Assignment update failed!";
                    ViewBag.SubMessage = "Invalid data inserted";
                    ViewBag.Image = "/assets/icons/error.svg";
                }
                return View("UserFeedback");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/delete-assignment/{id}")]
        [HttpDelete]
        public ActionResult DeleteAssignment(int id)
        {
            try
            {
                //the assignment is not deleted per se, it is just marked inactive so it is not visible in the FE
                int noOfRowsAffected = assignmentBusiness.MakeAssignmentInactive(id);

                if (noOfRowsAffected > 0)
                {
                    ViewBag.Message = "Assignment deleted successfully";
                    ViewBag.ResponseStyleClass = "text-success";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "Assignment deleted!";
                    ViewBag.SubMessage = "Your assignment is now deleted";
                    ViewBag.Image = "/assets/icons/success.svg";
                }
                else
                {
                    ViewBag.Message = "Assignment deletion failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to the assignment form";
                    ViewBag.ButtonLink = "/assignment/update-assignment/" + id;
                    ViewBag.PageTitle = "Assignment deletion failed!";
                    ViewBag.SubMessage = "There was a server error \ntry again later";
                    ViewBag.Image = "/assets/icons/error.svg";
                }
                return View("UserFeedback");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/user")]
        [HttpGet]
        public ActionResult GetAllAssignmentsForLoggedInUser()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<Assignment> assignments = assignmentBusiness.GetAllAssignmentsForUser(userId);

                if (assignments.Count > 0)
                {
                    ViewBag.Assignments = assignments;
                    ViewBag.ShowSolvedByUser = false;

                    return View("AllAssignments");
                }
                else
                {
                    ViewBag.Message = "No assignment found";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "No assignments found!";
                    ViewBag.SubMessage = "There were no assignments \nfor the given query";
                    ViewBag.Image = "/assets/icons/error.svg";
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("assignment/solved-by-user")]
        [HttpGet]
        public ActionResult GetAllAssignmentsSolvedByLoggedInUser()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<Assignment> solvedAssignments = assignmentBusiness.GetAllAssignmentsSolvedByUser(userId);

                if (solvedAssignments.Count > 0)
                {
                    ViewBag.Assignments = solvedAssignments;
                    ViewBag.ShowSolvedByUser = true;

                    return View("AllAssignments");
                }
                else
                {
                    ViewBag.Message = "No assignment found";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "No assignments found!";
                    ViewBag.SubMessage = "There were no assignments \nfor the given query";
                    ViewBag.Image = "/assets/icons/error.svg";
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }
    }
}
