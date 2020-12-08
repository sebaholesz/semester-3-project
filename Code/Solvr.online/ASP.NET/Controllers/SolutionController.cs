using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace webApi.Controllers
{
    [Authorize]
    public class SolutionController : Controller
    {

        public SolutionController()
        {

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
                int returnCode;
                string baseUrl = "http://localhost";

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = client.PostAsync(baseUrl, new StringContent(JsonConvert.SerializeObject(new { userId = userId, assignmentId = assignmentId }))).Result)
                    {
                        using (HttpContent content = res.Content)
                        {
                            returnCode = Convert.ToInt32(content.ReadAsStringAsync().Result);
                        }
                    }
                }

                /*
                 * 0 = hes neither author nor previous solver
                 * 1 = authorUserId = currentUserId
                 * 2 = solverId = currentUserId
                 */
                switch (returnCode)
                {
                    case 0:
                        Assignment assignment;
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage res = client.GetAsync(baseUrl + assignmentId).Result)
                            {
                                using (HttpContent content = res.Content)
                                {
                                    assignment = JsonConvert.DeserializeObject<Assignment>(content.ReadAsStringAsync().Result);
                                }
                            }

                            using (HttpResponseMessage res = client.PostAsync(baseUrl, new StringContent(JsonConvert.SerializeObject(new { userId = userId, assignmentId = assignmentId }))).Result)
                            {
                                using (HttpContent content = res.Content)
                                {
                                    returnCode = Convert.ToInt32(content.ReadAsStringAsync().Result);
                                }
                            }

                            using (HttpResponseMessage res = client.PostAsync(baseUrl, new StringContent(JsonConvert.SerializeObject(new { userId = userId, assignmentId = assignmentId }))).Result)
                            {
                                using (HttpContent content = res.Content)
                                {
                                    returnCode = Convert.ToInt32(content.ReadAsStringAsync().Result);
                                }
                            }
                        }
                       
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
                        return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
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
        [Route("solution/assignment/{assignmentId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSolution(IFormCollection collection, IFormFile files, int assignmentId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);
                    int queueOrder;

                    /*
                     * 0 = hes neither author nor previous solver
                     * 1 = authorUserId = currentUserId
                     * 2 = solverId = currentUserId
                     */
                    switch (returnCode)
                    {
                        case 0:
                            Solution solution;
                            if (files != null)
                            {
                                var dataStream = new MemoryStream();
                                await files.CopyToAsync(dataStream);

                                solution = new Solution(
                                    assignmentId,
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
                                    assignmentId,
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
                                ViewBag.ButtonLink = "/solution/assignment/" + assignmentId;
                                ViewBag.PageTitle = "Solution creation failed!";
                                ViewBag.SubMessage = "You are not in the queue";
                                ViewBag.Image = "/assets/icons/error.svg";
                            }
                            return View("UserFeedback");
                        case 1:
                            return Redirect("/assignment/update-assignment/" + assignmentId);
                        case 2:
                            return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
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
                        List<Solution> solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId);
                        if (solutions.Count > 0)
                        {
                            ViewBag.Solutions = solutions;
                            return View("DisplayAllSolutionsForAssignment");
                        }
                        else
                        {
                            ViewBag.Message = "No solutions found";
                            ViewBag.ResponseStyleClass = "text-danger";
                            ViewBag.ButtonText = "Go back to the solution form";
                            ViewBag.ButtonLink = "/solution/assignment/" + assignmentId;
                            ViewBag.PageTitle = "No solutions found!";
                            ViewBag.SubMessage = "No one has solved your assignment yet";
                            ViewBag.Image = "/assets/icons/error.svg";
                            return View("UserFeedback");
                        }
                    case 2:
                        return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
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
         * posted the assignment  
         * and is logged in
         */
        [Route("solution/choose-solution")]
        [HttpPut]
        public ActionResult ChooseSolutionPut([FromBody] string reqBody)
        {
            try
            {
                string[] reqBodyStringArray = reqBody.Split("*");
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
                        if (solutionBusiness.ChooseSolution(solutionId, assignmentId))
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
                        return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
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
        [Route("solution/solution-for-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayAcceptedSolutionByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Solution solution = solutionBusiness.GetSolutionByAssignmentId(assignmentId);
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
                    case 2:
                        return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
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
        * posted the solution  
        * and is logged in
        */
        [Route("solution/my-solution-for-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayMySolutionByAssignmentId(int assignmentId)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Solution solution = solutionBusiness.GetSolutionByAssignmentId(assignmentId);
                int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);
                Assignment solvedAssignment = assignmentBusiness.GetByAssignmentId(assignmentId);

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
                        return Redirect("/assignment/display-assignment/" + assignmentId);
                    case 2:
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
    }
}
