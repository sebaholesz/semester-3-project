using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace webApi.Controllers
{
    [Authorize]
    public class SolutionController : Controller
    {
        private readonly AssignmentBusiness assignmentBusiness;
        private readonly SolutionBusiness solutionBusiness;
        private readonly UserBusiness userBusiness;

        public SolutionController()
        {
            assignmentBusiness = new AssignmentBusiness();
            solutionBusiness = new SolutionBusiness();
            userBusiness = new UserBusiness();
        }

        [Route("solution/assignment/{id}")]
        [HttpGet]
        public ActionResult CreateSolution(int id)
        {
            try
            {
                Assignment assignment = assignmentBusiness.GetByAssignmentId(id);
                ViewBag.Assignment = assignment;
                ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(id).Count;

                ViewBag.Username = userBusiness.GetUserUsername(assignment.UserId);
                if (assignment.Anonymous)
                {
                    ViewBag.Name = "";
                }
                else
                {
                    ViewBag.Name = userBusiness.GetUserName(assignment.UserId);
                }
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
        public async Task<ActionResult> CreateSolution(IFormCollection collection, IFormFile files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Solution solution;
                    if (files != null)
                    {
                        var dataStream = new MemoryStream();
                        await files.CopyToAsync(dataStream);

                        solution = new Solution(
                            Convert.ToInt32(collection["Solution.AssignmentId"]),
                            collection["Solution.Description"],
                            DateTime.Now,
                            Convert.ToBoolean(collection["Solution.Anonymous"][0]),
                            dataStream.ToArray(),
                            User.FindFirstValue(ClaimTypes.NameIdentifier)
                        );
                        dataStream.Close();
                    }
                    else
                    {
                        solution = new Solution(
                            Convert.ToInt32(collection["Solution.AssignmentId"]),
                            collection["Solution.Description"],
                            DateTime.Now,
                            Convert.ToBoolean(collection["Solution.Anonymous"][0]),
                            User.FindFirstValue(ClaimTypes.NameIdentifier)
                        );
                    }


                    int queueOrder = solutionBusiness.CreateSolution(solution);

                    if (queueOrder > 0)
                    {
                        ViewBag.Message = "Solution created successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                        ViewBag.ButtonText = "Go back to homepage";
                        ViewBag.ButtonLink = "/";
                        ViewBag.PageTitle = "Solution created!";
                        ViewBag.SubMessage = "You are number " + queueOrder + " in the queue";
                        ViewBag.Image = "/assets/icons/success.svg";
                    }
                    else if (queueOrder == -2)
                    {
                        ViewBag.Message = "Solution creation failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to the solution form";
                        ViewBag.ButtonLink = "/solution/assignment/" + collection["Solution.AssignmentId"];
                        ViewBag.PageTitle = "Solution creation failed!";
                        ViewBag.SubMessage = "You already solved this assignment";
                        ViewBag.Image = "/assets/icons/error.svg";
                    }
                    else if (queueOrder == -3)
                    {
                        ViewBag.Message = "Solution creation failed";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to the solution form";
                        ViewBag.ButtonLink = "/solution/assignment/" + collection["Solution.AssignmentId"];
                        ViewBag.PageTitle = "Solution creation failed!";
                        ViewBag.SubMessage = "You cannot solve your own assignment";
                        ViewBag.Image = "/assets/icons/error.svg";
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
        public ActionResult ChooseSolutionGet(int assignmentId)
        {
            ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId);
            return View("DisplayAllSolutionsForAssignment");
        }

        [Route("solution/choose-solution")]
        [HttpPut]
        public ActionResult ChooseSolutionPut([FromBody] int solutionId)
        {
            try
            {
                int noOfRowsAffected = solutionBusiness.ChooseSolution(solutionId);

                if(noOfRowsAffected == 1)
                {
                    //display solution here
                    //return View("");
                    ViewBag.Message = "Solution accepted";
                    ViewBag.ResponseStyleClass = "text-success";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "Solution accepted!";
                    ViewBag.SubMessage = "The solution is now accepted \nand is waiting for you";
                    ViewBag.Image = "/assets/icons/success.svg";
                }
                else
                {
                    //return error here
                    ViewBag.Message = "Solution acceptation failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "Solution acceptation failed!";
                    ViewBag.SubMessage = "There was an internal error \nwhile processing your request";
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

        [Route("solution/user-solution-for-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplaySolutionForUserByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Solution userSolution = solutionBusiness.GetSolutionForUserByAssignmentId(userId, assignmentId);

                if (!userSolution.Equals(null))
                {
                    Assignment solvedAssignment = assignmentBusiness.GetByAssignmentId(assignmentId);
                    ViewBag.Assignment = solvedAssignment;
                    ViewBag.Solution = userSolution;

                    ViewBag.Username = userBusiness.GetUserUsername(solvedAssignment.UserId);
                    if (solvedAssignment.Anonymous)
                    {
                        ViewBag.Name = "";
                    }
                    else
                    {
                        ViewBag.Name = userBusiness.GetUserName(solvedAssignment.UserId);
                    }

                    return View("DisplaySolution");
                }
                else
                {
                    ViewBag.Message = "Could not find your solution";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to homepage";
                    ViewBag.ButtonLink = "/";
                    ViewBag.PageTitle = "Could not find your solution!";
                    ViewBag.SubMessage = "You did not \nsolve this assignment";
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
