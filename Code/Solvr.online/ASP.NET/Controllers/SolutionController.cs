using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

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

        /*can be accessed by everybody who 
         * hasnt posted the assignment  
         * and hasnt solved it yet 
         * and is logged in
         */
        [Route("solution/assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult CreateSolution(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

                /*
                 * 0 = hes neither author nor previous solver
                 * 1 = authorUserId = currentUserId
                 * 2 = solverId = currentUserId
                 */
                switch (returnCode)
                {
                    case 0:
                        Assignment assignment = assignmentBusiness.GetByAssignmentId(assignmentId);
                        ViewBag.Assignment = assignment;
                        ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId).Count;
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
                    case 1:
                        return Redirect("/assignment/update-assignment/" + assignmentId);
                    case 2:
                        return Redirect("/solution/user-solution-for-assignment/" + assignmentId);
                    default:
                        throw new Exception("Internal server error");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * hasnt posted the assignment  
         * and hasnt solved it yet 
         * and is logged in
         */
        [Route("solution/assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSolution(IFormCollection collection, IFormFile files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int assignmentId = Convert.ToInt32(collection["Solution.AssignmentId"]);
                    int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId , userId);
                    int queueOrder;

                    /*
                     * 1 = OK
                     * -1 = authorUserId = currentUserId
                     * -2 = solverId = currentUserId
                     */
                    switch (returnCode)
                    {
                        case 1:
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
                            queueOrder = solutionBusiness.CreateSolution(solution);

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
                            return View("UserFeedback");
                        case -1:
                            return Redirect("/assignment/update-assignment/" + assignmentId);
                        case -2:
                            return Redirect("/solution/user-solution-for-assignment/" + assignmentId);
                        default:
                            throw new Exception("Internal server error");
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
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment  
         * and is logged in
         */
        [Route("solution/solution-queue/{assignmentId}")]
        [HttpGet]
        public ActionResult ChooseSolutionGet(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

                /*
                 * 0 = hes neither author nor previous solver
                 * 1 = authorUserId = currentUserId
                 * 2 = solverId = currentUserId
                 */
                switch (returnCode)
                {
                    case 0:
                        return Redirect("/assignment/display-assignment/" + assignmentId);
                    case 1:
                        ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId);
                        return View("DisplayAllSolutionsForAssignment");
                    case 2:
                        return Redirect("/solution/user-solution-for-assignment/" + assignmentId);
                    default:
                        throw new Exception("Internal server error");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment  
         * and is logged in
         */
        [Route("solution/choose-solution")]
        [HttpPut]
        public ActionResult ChooseSolutionPut([FromBody] string reqBody)
        {
            try
            {
                string[] reqBodyStringArray =  reqBody.Split("*");
                int solutionId = Convert.ToInt32(reqBodyStringArray[0]);
                int assignmentId = Convert.ToInt32(reqBodyStringArray[1]);
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

                /*
                 * 0 = hes neither author nor previous solver
                 * 1 = authorUserId = currentUserId
                 * 2 = solverId = currentUserId
                 */
                switch (returnCode)
                {
                    case 0:
                        return Redirect("/assignment/display-assignment/" + assignmentId);
                    case 1:
                        if(solutionBusiness.ChooseSolution(solutionId, assignmentId))
                        {
                            break;
                        }
                        else
                        {
                            ViewBag.Message = "Solution acceptation failed";
                            ViewBag.ResponseStyleClass = "text-danger";
                            ViewBag.ButtonText = "Go back to homepage";
                            ViewBag.ButtonLink = "/";
                            ViewBag.PageTitle = "Solution acceptation failed!";
                            ViewBag.SubMessage = "There was an internal error \nwhile processing your request";
                            ViewBag.Image = "/assets/icons/error.svg";
                        }
                        return View("UserFeedback");
                    case 2:
                        return Redirect("/solution/user-solution-for-assignment/" + assignmentId);
                    default:
                        throw new Exception("Internal server error");
                }

                Solution solution = solutionBusiness.GetSolutionByAssignmentId(assignmentId);

                Assignment solvedAssignment = assignmentBusiness.GetByAssignmentId(assignmentId);
                ViewBag.Assignment = solvedAssignment;
                ViewBag.Solution = solution;

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
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment  
         * and is logged in
         */
        [Route("solution/user-solution-for-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayAcceptedSolutionByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Solution solution = solutionBusiness.GetSolutionByAssignmentId(assignmentId);

                Assignment solvedAssignment = assignmentBusiness.GetByAssignmentId(assignmentId);
                if (!solvedAssignment.IsActive)
                {
                    ViewBag.Assignment = solvedAssignment;
                    ViewBag.Solution = solution;

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
                    throw new Exception("The assignment is not closed yet");
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
