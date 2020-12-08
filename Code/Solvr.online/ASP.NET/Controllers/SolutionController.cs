using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
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
                string baseUrl = "http://localhost:44316/apiV1/";
                string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                using (HttpClient client = new HttpClient())
                {
                    returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser,new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);
                }

                /*
                 * 0 = hes neither author nor previous solver
                 * 1 = authorUserId = currentUserId
                 * 2 = solverId = currentUserId
                 */
                switch (returnCode)
                {
                    case 0:
                        using (HttpClient client = new HttpClient())
                        {
                            string urlCompleteAssignmentData = "https://www.localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                            AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
                            ViewBag.Assignment = asu.Assingment;
                            //TODO get number of solutions we could parse to front end
                            //ViewBag.SolutionCount = JsonConvert.DeserializeObject<int>((client.GetAsync(url).Result).Content.ReadAsStringAsync().Result);
                            ViewBag.User = asu.User;
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
                    int returnCode;
                    string baseUrl = "http://localhost:44316/apiV1/";
                    string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                    using (HttpClient client = new HttpClient())
                    {
                        returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser, new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);
                    }

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

                            int queueOrder;
                            using (HttpClient client = new HttpClient())
                            {
                                string urlCreateSolution = "https://www.localhost:44316/apiV1/solution/" + assignmentId;
                                queueOrder = Convert.ToInt32((client.PostAsync(urlCreateSolution, new StringContent(JsonConvert.SerializeObject(solution))).Result).Content.ReadAsStringAsync().Result);
                            }

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
                int returnCode;
                string baseUrl = "http://localhost:44316/apiV1/";
                string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                using (HttpClient client = new HttpClient())
                {
                    returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser, new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);
                }

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
                        using (HttpClient client = new HttpClient())
                        {
                            List<Solution> solutions = JsonConvert.DeserializeObject<List<Solution>>(
                            (client.GetAsync("https://localhost:44316/apiV1/academiclevel").Result).Content.ReadAsStringAsync().Result);
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
                using (HttpClient client = new HttpClient())
                {
                    string[] reqBodyStringArray = reqBody.Split("*");
                    int solutionId = Convert.ToInt32(reqBodyStringArray[0]);
                    int assignmentId = Convert.ToInt32(reqBodyStringArray[1]);

                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int returnCode;
                    string baseUrl = "http://localhost:44316/apiV1/";
                    string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                    returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser, new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);


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
                            if (Convert.ToBoolean((client.PostAsync(urlCheckUser, new StringContent(JsonConvert.SerializeObject(new { solutionId = solutionId, assignmentId = assignmentId }))).Result).Content.ReadAsStringAsync().Result))
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

                    Solution solution = JsonConvert.DeserializeObject<Solution>((client.GetAsync("https://localhost:44316/apiV1/solution/"+assignmentId).Result).Content.ReadAsStringAsync().Result);

                    string urlCompleteAssignmentData = "https://www.localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                    AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
                    ViewBag.Assignment = asu.Assingment;
                    ViewBag.Solution = solution;
                    ViewBag.User = asu.User;

                    return View("DisplaySolution");
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
        [Route("solution/solution-for-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayAcceptedSolutionByAssignmentId(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Solution solution = JsonConvert.DeserializeObject<Solution>((client.GetAsync("https://localhost:44316/apiV1/solution/" + assignmentId).Result).Content.ReadAsStringAsync().Result);

                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int returnCode;
                    string baseUrl = "http://localhost:44316/apiV1/";
                    string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                    returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser, new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);

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
                            //TODO check if active in lower layers
                            //Assignment solvedAssignment = assignmentBusiness.GetByAssignmentId(assignmentId);
                            //if (!solvedAssignment.IsActive)
                            //{

                                string urlCompleteAssignmentData = "https://www.localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                                AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
                                ViewBag.Assignment = asu.Assingment;
                                ViewBag.Solution = solution;
                                ViewBag.User = asu.User;

                                return View("DisplaySolution");
                            //}
                            //else
                            //{
                            //    throw new Exception("The assignment is not closed yet");
                            //}
                        case 2:
                            return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
                        default:
                            throw new Exception("Internal server error");
                    }
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
                using (HttpClient client = new HttpClient())
                {
                    Solution solution = JsonConvert.DeserializeObject<Solution>((client.GetAsync("https://localhost:44316/apiV1/solution/" + assignmentId).Result).Content.ReadAsStringAsync().Result);

                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    int returnCode;
                    string baseUrl = "http://localhost:44316/apiV1/";
                    string urlCheckUser = baseUrl + "check-user-vs-assignment/" + assignmentId;

                    returnCode = Convert.ToInt32((client.PostAsync(urlCheckUser, new StringContent(userId)).Result).Content.ReadAsStringAsync().Result);

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
                            string urlCompleteAssignmentData = "https://www.localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                            AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
                            ViewBag.Assignment = asu.Assingment;
                            ViewBag.Solution = solution;
                            ViewBag.User = asu.User;

                            return View("DisplaySolution");
                        default:
                            throw new Exception("Internal server error");
                    }
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
