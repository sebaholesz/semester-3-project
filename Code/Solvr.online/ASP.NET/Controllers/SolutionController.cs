using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace webApi.Controllers
{
    public class SolutionController : Controller
    {
        private readonly AssignmentBusiness assignmentBusiness;
        private readonly SolutionBusiness solutionBusiness;

        public SolutionController()
        {
            assignmentBusiness = new AssignmentBusiness();
            solutionBusiness = new SolutionBusiness();
        }

        [Route("solution/assignment/{id}")]
        [HttpGet]
        public ActionResult CreateSolution(int id)
        {
            try
            {
                ViewBag.Assignment = assignmentBusiness.GetByAssignmentId(id);
                return View("CreateSolution");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        [Route("solution/assignment/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSolution(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Solution solution = new Solution(
                        Convert.ToInt32(collection["Solution.AssignmentId"]),
                        12,
                        collection["Solution.Description"],
                        DateTime.Now,
                        Convert.ToBoolean(collection["Solution.Anonymous"][0])
                        );

                    int queueOrder = solutionBusiness.CreateSolution(solution);
                    
                    if (queueOrder > 0)
                    {
                        ViewBag.Message = "Solution created successfully";
                        ViewBag.ResponseStyleClass ="text-success";
                        ViewBag.ButtonText = "Go back to homepage";
                        ViewBag.ButtonLink = "/";
                        ViewBag.PageTitle = "Solution created!";
                        ViewBag.SubMessage = "You are number " + queueOrder + " in the queue";
                        ViewBag.Image = "/assets/icons/success.svg";
                    }
                    else
                    {
                        ViewBag.Message = "Solution creation failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to the solution form";
                        ViewBag.ButtonLink = "/solution/assignment/" + collection["Solution.AssignmentId"];
                        ViewBag.PageTitle = "Solution creation failed!";
                        ViewBag.SubMessage = "You are not in the queue";
                        ViewBag.Image = "/assets/icons/error.svg";
                    }
                }
                else
                {
                    ViewBag.Message = "Solution creation failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to the solution form";
                    ViewBag.ButtonLink = "/solution/assignment/" + collection["Solution.AssignmentId"];
                    ViewBag.PageTitle = "Solution creation failed!";
                    ViewBag.SubMessage = "Invalid data inserted \nyou are not in the queue";
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

        [Route("solution/solution-queue/{assignmentId}")]
        [HttpGet]
        public ActionResult ChooseSolution(int assignmentId)
        {
            ViewBag.Solutions = solutionBusiness.GetSolutionsTimestampOrderedByAssignmentId(assignmentId);
            return View("DisplayAllSolutionsForAssignment");
        }
    }
}
