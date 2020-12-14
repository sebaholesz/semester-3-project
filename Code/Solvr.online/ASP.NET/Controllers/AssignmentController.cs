using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MimeDetective;
using ASP.NET.Controllers;

namespace webApi.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AssignmentController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        /*can be accessed by everybody who 
         * is logged in
         */
        [Route("assignment/create-assignment")]
        [HttpGet]
        public ActionResult CreateAssignment()
        {   
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetAllAcademicLevels = "https://localhost:44316/apiV1/academiclevel";
                    string urlGetAllSubjects = "https://localhost:44316/apiV1/subject";
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    string urlGetUserCredit = "https://localhost:44316/apiV1/user/get-credit/" + userId;


                    HttpResponseMessage academicLevelsRM = (client.GetAsync(urlGetAllAcademicLevels).Result);
                    HttpResponseMessage subjectsRM = (client.GetAsync(urlGetAllSubjects).Result);
                    HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);


                    if (academicLevelsRM.IsSuccessStatusCode && subjectsRM.IsSuccessStatusCode && urlGetUserCreditRM.IsSuccessStatusCode)
                    {
                        ViewBag.AcademicLevels = JsonConvert.DeserializeObject<List<string>>(academicLevelsRM.Content.ReadAsStringAsync().Result);
                        ViewBag.Subjects = JsonConvert.DeserializeObject<List<string>>(subjectsRM.Content.ReadAsStringAsync().Result);
                        ViewBag.Credits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);
                        return View("CreateAssignment");
                    }
                    else
                    {
                        throw new Exception("Internal server error");
                    }
                }
            }
            catch (Exception e)
            {
                if(e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }
        
        /*can be accessed by everybody who 
         * is logged in
         */
        [Route("assignment/create-assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAssignmentAsync(IFormCollection collection, IFormFile files)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (ModelState.IsValid)
                    {
                        User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                        client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                        Assignment assignment = new Assignment();
                        assignment.Title = collection["Title"];
                        assignment.Description = collection["Description"];
                        assignment.Price = Convert.ToInt32(collection["Price"]);
                        assignment.Deadline = Convert.ToDateTime(collection["Deadline"]);
                        assignment.Anonymous = Convert.ToBoolean(collection["Anonymous"][0]);
                        assignment.AcademicLevel = collection["AcademicLevel"];
                        assignment.Subject = collection["Subject"];
                        assignment.UserId = user.Id;

                        //TODO here get the file size and file type and add restrictions
                        if (files != null)
                        {
                            var dataStream = new MemoryStream();
                            await files.CopyToAsync(dataStream);
                            assignment.AssignmentFile = dataStream.ToArray();
                            dataStream.Close();
                        }

                        string urlGetUserCredit = "https://localhost:44316/apiV1/user/get-credit/" + user.Id;
                        HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);
                        int userCredits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);

                        string urlCreateAssignment = "https://localhost:44316/apiV1/assignment";
                        if (assignment.Price <= userCredits)
                        {
                            HttpResponseMessage createAssignmentRM = client.PostAsync(urlCreateAssignment, new StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")).Result;

                            if (createAssignmentRM.IsSuccessStatusCode)
                            {
                                int lastUsedId = createAssignmentRM.Content.ReadAsAsync<int>().Result;
                                ViewBag.Message = "Assignment created successfully";
                                ViewBag.ResponseStyleClass = "text-success";
                                ViewBag.ButtonText = "Display your assignment";
                                ViewBag.ButtonLink = "/assignment/display-assignment/" + lastUsedId;
                                ViewBag.PageTitle = "Assignment created!";
                                ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                                ViewBag.Image = "/assets/icons/success.svg";
                            }
                            else
                            {
                                throw new Exception(createAssignmentRM.ReasonPhrase);
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid data inserted");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid data inserted");
                    }
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * hasnt posted the assignment  
         * and hasnt solved it yet 
         * and is logged in
         */
        [Route("assignment/display-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlCheckUser = $"https://localhost:44316/apiV1/check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.PostAsync(urlCheckUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

                    if (returnCodeRM.IsSuccessStatusCode)
                    {
                        int returnCode = returnCodeRM.Content.ReadAsAsync<int>().Result;

                        /*
                         * 0 = hes neither author nor previous solver
                         * 1 = authorUserId = currentUserId
                         * 2 = solverId = currentUserId
                         */
                        switch (returnCode)
                        {
                            case 0:
                                string urlCompleteAssignmentData = "https://localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                                HttpResponseMessage asuRM = client.GetAsync(urlCompleteAssignmentData).Result;
                                if (asuRM.IsSuccessStatusCode)
                                {
                                    AssignmentSolutionUser asu = asuRM.Content.ReadAsAsync<AssignmentSolutionUser>().Result;
                                    ViewBag.Assignment = asu.Assignment;
                                    ViewBag.User = asu.User;

                                    string urlCountOfSolutions = "https://localhost:44316/apiV1/solution/count-by-assignmentId/" + assignmentId;
                                    HttpResponseMessage solutionCountRM = client.GetAsync(urlCountOfSolutions).Result;
                                    if (solutionCountRM.IsSuccessStatusCode)
                                    {
                                        ViewBag.SolutionCount = solutionCountRM.Content.ReadAsStringAsync().Result;
                                    }
                                    else
                                    {
                                        ViewBag.SolutionCount = "Could not load";
                                    }
                                    return View("DisplayAssignment");
                                }
                                else
                                {
                                    throw new Exception("Could not find the assignment");
                                }
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
                        throw new Exception("Internal server error");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        //can be accessed by everybody
        [AllowAnonymous]
        [Route("assignment/display-assignments-page/{pageNumber}")]
        [HttpGet]
        public ActionResult DisplayAllAssignments(int pageNumber)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                        client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                        // MAYBE TODO counts of answers to all assignments in assignment Cards
                        string urlGetAllAssignments = $"https://localhost:44316/apiV1/assignment/page-all-active-not-posted-by-user/{pageNumber}";

                        HttpResponseMessage assignmentsNotPostedByUserRM = client.PostAsync(urlGetAllAssignments, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;
                        if (assignmentsNotPostedByUserRM.IsSuccessStatusCode)
                        {
                            ViewBag.Assignments = assignmentsNotPostedByUserRM.Content.ReadAsAsync<List<Assignment>>().Result;
                            Page page = JsonConvert.DeserializeObject<Page>(assignmentsNotPostedByUserRM.Headers.GetValues("PagingHeaders").FirstOrDefault());
                            ViewBag.NextPage = pageNumber + 1;
                            ViewBag.PreviousPage = pageNumber - 1;
                            ViewBag.Link = "/assignment/display-assignments-page/";
                            ViewBag.PreviousEnable = page.PreviousPage == true ? "" : "disabled";
                            ViewBag.NextEnable = page.NextPage == true ? "" : "disabled";
                            ViewBag.PageNumber = pageNumber;
                            ViewBag.TotalPages = page.TotalPages;
                            //string urlCountOfSolutions = "https://localhost:44316/apiV1/solution/count-by-assignmentId/" + assignmentId;
                            //HttpResponseMessage solutionCountRM = client.GetAsync(urlCountOfSolutions).Result;
                            //if (solutionCountRM.IsSuccessStatusCode)
                            //{
                            //    ViewBag.SolutionCount = solutionCountRM.Content.ReadAsStringAsync().Result;
                            //}
                            //else
                            //{
                            //    ViewBag.SolutionCount = "Could not load";
                            //}
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
                    else
                    {
                        string urlGetAllAssignments = "https://localhost:44316/apiV1/assignment/page-all-active/" + pageNumber;
                        HttpResponseMessage allAssignmentsRM = client.GetAsync(urlGetAllAssignments).Result;
                        if (allAssignmentsRM.IsSuccessStatusCode)
                        {
                            ViewBag.Assignments = allAssignmentsRM.Content.ReadAsAsync<List<Assignment>>().Result;
                            Page page = JsonConvert.DeserializeObject<Page>(allAssignmentsRM.Headers.GetValues("PagingHeaders").FirstOrDefault());
                            ViewBag.NextPage = pageNumber + 1;
                            ViewBag.PreviousPage = pageNumber - 1;
                            ViewBag.Link = "/assignment/display-assignments-page/";
                            ViewBag.PreviousEnable = page.PreviousPage == true ? "" : "disabled";
                            ViewBag.NextEnable = page.NextPage == true ? "" : "disabled";
                            ViewBag.PageNumber = pageNumber;
                            ViewBag.TotalPages = page.TotalPages;
                            //string urlCountOfSolutions = "https://localhost:44316/apiV1/solution/count-by-assignmentId/" + assignmentId;
                            //HttpResponseMessage solutionCountRM = client.GetAsync(urlCountOfSolutions).Result;
                            //if (solutionCountRM.IsSuccessStatusCode)
                            //{
                            //    ViewBag.SolutionCount = solutionCountRM.Content.ReadAsStringAsync().Result;
                            //}
                            //else
                            //{
                            //    ViewBag.SolutionCount = "Could not load";
                            //}
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
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment
         * and only if the assignment is still active
         */
        [Route("assignment/update-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult UpdateAssignment(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlCheckUser = $"https://localhost:44316/apiV1/check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.PostAsync(urlCheckUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

                    if (returnCodeRM.IsSuccessStatusCode)
                    {
                        int returnCode = returnCodeRM.Content.ReadAsAsync<int>().Result;

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
                                string urlGetAssignment = "https://localhost:44316/apiV1/assignment/" + assignmentId;
                                HttpResponseMessage getAssignmentRM = client.GetAsync(urlGetAssignment).Result;
                                
                                if(getAssignmentRM.IsSuccessStatusCode)
                                {
                                    Assignment assignment = getAssignmentRM.Content.ReadAsAsync<Assignment>().Result;
                                    if (assignment.IsActive)
                                    {
                                        ViewBag.Assignment = assignment;
                                        ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
                                        ViewBag.AcademicLevel = JsonConvert.DeserializeObject<List<string>>(client.GetAsync("https://localhost:44316/apiV1/academiclevel").Result.Content.ReadAsStringAsync().Result);
                                        ViewBag.Subject = JsonConvert.DeserializeObject<List<string>>(client.GetAsync("https://localhost:44316/apiV1/subject").Result.Content.ReadAsStringAsync().Result);
                                        string bitString = BitConverter.ToString(assignment.Timestamp);
                                        ViewBag.Timestamp  = bitString;
                                        return View("UpdateAssignment");
                                    }
                                    else
                                    {
                                        return Redirect("/solution/solution-for-assignment/" + assignmentId);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Could not find the assignment");
                                }
                            case 2:
                                return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
                            default:
                                throw new Exception("Internal server error");
                        }
                    }
                    else
                    {
                        throw new Exception("Internal server error");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment
         * and only if the assignment is still active
         */
        [Route("assignment/update-assignment/{assignmentId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAssignment(IFormCollection collection, int assignmentId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                        client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                        string urlCheckUser = $"https://localhost:44316/apiV1/check-user-vs-assignment/{assignmentId}";
                        HttpResponseMessage returnCodeRM = client.PostAsync(urlCheckUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

                        if (returnCodeRM.IsSuccessStatusCode)
                        {
                            int returnCode = returnCodeRM.Content.ReadAsAsync<int>().Result;

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
                                    Assignment assignment = new Assignment();

                                    assignment.Title = collection["Title"];
                                    assignment.Description = collection["Description"];
                                    assignment.Price = Convert.ToInt32(collection["Price"]);
                                    assignment.Deadline = DateTime.Parse(collection["Deadline"]);
                                    assignment.Anonymous = Convert.ToBoolean(collection["Anonymous"][0]);
                                    assignment.AcademicLevel = collection["AcademicLevel"];
                                    assignment.Subject = collection["Subject"];

                                    //code from Stackoverflow
                                    String[] arr = collection["Timestamp"].ToString().Split('-');
                                    byte[] array = new byte[arr.Length];
                                    for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);

                                    assignment.Timestamp = array;


                                    //TODO check if the file should be updated
                                    //assignment.AssignmentFile = Encoding.ASCII.GetBytes(collection["AssignmentFile"]);

                                    string urlUpdateAssignment = "https://localhost:44316/apiV1/assignment/" + assignmentId;
                                    HttpResponseMessage updateAssignmentRM = client.PutAsync(urlUpdateAssignment, 
                                        new StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")).Result;

                                    if(updateAssignmentRM.IsSuccessStatusCode)
                                    {
                                        //TODO notify all solvers of the changes

                                        ViewBag.Message = "Assignment updated successfully";
                                        ViewBag.ResponseStyleClass = "text-success";
                                        ViewBag.ButtonText = "Display your assignment";
                                        ViewBag.ButtonLink = "/assignment/display-assignment/" + assignmentId;
                                        ViewBag.PageTitle = "Assignment updated!";
                                        ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                                        ViewBag.Image = "/assets/icons/success.svg";
                                        return View("UserFeedback");
                                    }
                                    else
                                    {
                                        throw new Exception(updateAssignmentRM.ReasonPhrase);
                                    }
                                case 2:
                                    return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
                                default:
                                    throw new Exception("Internal server error");
                            }
                        }
                        else
                        {
                            throw new Exception("Internal server error");
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Assignment update failed";
                    ViewBag.ResponseStyleClass = "text-danger";
                    ViewBag.ButtonText = "Go back to the assignment form";
                    ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
                    ViewBag.PageTitle = "Assignment update failed!";
                    ViewBag.SubMessage = "Invalid data inserted";
                    ViewBag.Image = "/assets/icons/error.svg";
                }
                return View("UserFeedback");
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        //*can be accessed by everybody who 
        // * posted the assignment
        // * and only if the assignment is still active
        // */
        [Route("assignment/delete-assignment/{assignmentId}")]
        [HttpPut]
        public ActionResult DeleteAssignment(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlCheckUser = $"https://localhost:44316/apiV1/check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.PostAsync(urlCheckUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

                    if (returnCodeRM.IsSuccessStatusCode)
                    {
                        int returnCode = returnCodeRM.Content.ReadAsAsync<int>().Result;

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
                                string urlMakeInactive = "https://localhost:44316/apiV1/assignment/inactive/" + assignmentId;
                                HttpResponseMessage makeInactiveRM = client.PutAsync(urlMakeInactive, null).Result;

                                if (makeInactiveRM.IsSuccessStatusCode)
                                {
                                    ViewBag.Message = "Assignment deleted successfully";
                                    ViewBag.ResponseStyleClass = "text-success";
                                    ViewBag.ButtonText = "Go back to homepage";
                                    ViewBag.ButtonLink = "/";
                                    ViewBag.PageTitle = "Assignment deleted!";
                                    ViewBag.SubMessage = "Your assignment is now deleted";
                                    ViewBag.Image = "/assets/icons/success.svg";
                                    return View("UserFeedback");

                                }
                                else
                                {
                                    throw new Exception(makeInactiveRM.ReasonPhrase);
                                }
                            case 2:
                                return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
                            default:
                                throw new Exception("Internal server error");
                        }
                    }
                    else
                    {
                        throw new Exception("Internal server error");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * posted the assignment
         */
        [Route("assignment/page-user/{pageNumber}")]
        [HttpGet]
        public ActionResult GetAllAssignmentsForLoggedInUser(int pageNumber)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetAllAssignmentsForLoggedInUser = "https://localhost:44316/apiV1/assignment/page-user/" + pageNumber;
                    HttpResponseMessage getAllAssignmentsForLoggedInUserRM = client.PostAsync(urlGetAllAssignmentsForLoggedInUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;
                    
                    if(getAllAssignmentsForLoggedInUserRM.IsSuccessStatusCode)
                    {
                        List<Assignment> assignments = getAllAssignmentsForLoggedInUserRM.Content.ReadAsAsync<List<Assignment>>().Result;
                        Page page = JsonConvert.DeserializeObject<Page>(getAllAssignmentsForLoggedInUserRM.Headers.GetValues("PagingHeaders").FirstOrDefault());
                        ViewBag.NextPage = pageNumber + 1;
                        ViewBag.PreviousPage = pageNumber - 1;
                        ViewBag.Link = "/assignment/page-user/";
                        ViewBag.PreviousEnable = page.PreviousPage == true ? "" : "disabled";
                        ViewBag.NextEnable = page.NextPage == true ? "" : "disabled";
                        ViewBag.PageNumber = pageNumber;
                        ViewBag.TotalPages = page.TotalPages;
                        ViewBag.Assignments = assignments;
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
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * solved the assignment
         */
        [Route("assignment/solved-by-user")]
        [HttpGet]
        public ActionResult GetAllAssignmentsSolvedByLoggedInUser()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetAllAssignmentsSolvedByLoggedInUser = "https://localhost:44316/apiV1/assignment/solved-by-user";
                    HttpResponseMessage getAllAssignmentsSolvedByLoggedInUserRM = client.PostAsync(urlGetAllAssignmentsSolvedByLoggedInUser, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

                    if (getAllAssignmentsSolvedByLoggedInUserRM.IsSuccessStatusCode)
                    {
                        List<Assignment> solvedAssignments = getAllAssignmentsSolvedByLoggedInUserRM.Content.ReadAsAsync<List<Assignment>>().Result;

                        ViewBag.Assignments = solvedAssignments;
                        return View("AllAssignments");
                    }
                    else
                    {
                        ViewBag.Message = "No solutions found";
                        ViewBag.ResponseStyleClass = "text-danger";
                        ViewBag.ButtonText = "Go back to homepage";
                        ViewBag.ButtonLink = "/";
                        ViewBag.PageTitle = "No soluitons found!";
                        ViewBag.SubMessage = "You have not solved \nany assignments yet!";
                        ViewBag.Image = "/assets/icons/error.svg";
                        return View("UserFeedback");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        /*can be accessed by everybody who 
         * is logged in
         */
        [Route("assignment/download-file/{assignmentId}")]
        [HttpGet]
        public ActionResult DownloadFile(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetFileForDownload = $"https://localhost:44316/apiV1/assignment/get-file/{assignmentId}";
                    HttpResponseMessage getFileForDownloadRM = client.GetAsync(urlGetFileForDownload).Result;

                    if (getFileForDownloadRM.IsSuccessStatusCode)
                    {
                        byte[] fileContent = getFileForDownloadRM.Content.ReadAsAsync<byte[]>().Result;
                        string extension = fileContent.GetFileType().Extension;
                        string fileName = $"{assignmentId}-assignment-file_{DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year}.{extension}";
                        return File(fileContent,  "application/force-download" , fileName);
                    }
                    else
                    {
                        throw new Exception("Could not download the file");
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }
    }
}
