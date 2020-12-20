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
using System.Net.Http;
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
        private readonly string _baseUrl;

        public AssignmentController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _baseUrl = "https://localhost:44316/apiV1/";
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

                    string urlGetAllAcademicLevels = _baseUrl + "academiclevel";
                    string urlGetAllSubjects = _baseUrl + "subject";
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    string urlGetUserCredit = _baseUrl + "user/get-credit";


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

                        if (files != null)
                        {
                            var dataStream = new MemoryStream();
                            await files.CopyToAsync(dataStream);
                            assignment.AssignmentFile = dataStream.ToArray();
                            dataStream.Close();
                        }

                        string urlGetUserCredit = _baseUrl + "user/get-credit";
                        HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);
                        int userCredits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);

                        string urlCreateAssignment = _baseUrl + "assignment";
                        if (assignment.Price <= userCredits)
                        {
                            HttpResponseMessage createAssignmentRM = client.PostAsync(urlCreateAssignment, new StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")).Result;

                            if (createAssignmentRM.IsSuccessStatusCode)
                            {
                                int lastUsedId = createAssignmentRM.Content.ReadAsAsync<int>().Result;
                                TempData["Message"] = "Assignment created successfully";
                                TempData["ResponseStyleClass"] = "text-success";
                                TempData["ButtonText"] = "Display your assignment";
                                TempData["ButtonLink"] = "/assignment/display-assignment/" + lastUsedId;
                                TempData["PageTitle"] = "Assignment created!";
                                TempData["SubMessage"] = "Your assignment now waits for solvers to solve it";
                                TempData["Image"] = "/assets/icons/success.svg";
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
                    return Redirect("/success");
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

                    string urlCheckUser = _baseUrl + $"check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.GetAsync(urlCheckUser).Result;

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
                                string urlCompleteAssignmentData = _baseUrl + "assignment/complete-data/" + assignmentId;
                                HttpResponseMessage asuRM = client.GetAsync(urlCompleteAssignmentData).Result;
                                if (asuRM.IsSuccessStatusCode)
                                {
                                    AssignmentSolutionUser asu = asuRM.Content.ReadAsAsync<AssignmentSolutionUser>().Result;
                                    ViewBag.Assignment = asu.Assignment;
                                    ViewBag.User = asu.User;

                                    string urlCountOfSolutions = _baseUrl + "solution/count-by-assignmentId/" + assignmentId;
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

                        string urlGetAllAssignments = _baseUrl + $"assignment/page-all-active-not-posted-by-user/{pageNumber}";

                        HttpResponseMessage assignmentsNotPostedByUserRM = client.GetAsync(urlGetAllAssignments).Result;
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
                            return View("AllAssignments");
                        }
                        else
                        {
                            TempData["Message"] = "No assignment found";
                            TempData["ResponseStyleClass"] = "text-danger";
                            TempData["ButtonText"] = "Go back to homepage";
                            TempData["ButtonLink"] = "/";
                            TempData["PageTitle"] = "No assignments found!";
                            TempData["SubMessage"] = "There were no assignments \nfor the given query";
                            TempData["Image"] = "/assets/icons/error.svg";
                            return Redirect("/nothing-found");
                        }
                    }
                    else
                    {
                        string urlGetAllAssignments = _baseUrl + "assignment/page-all-active/" + pageNumber;
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
                            return View("AllAssignments");
                        }
                        else
                        {
                            TempData["Message"] = "No assignment found";
                            TempData["ResponseStyleClass"] = "text-danger";
                            TempData["ButtonText"] = "Go back to homepage";
                            TempData["ButtonLink"] = "/";
                            TempData["PageTitle"] = "No assignments found!";
                            TempData["SubMessage"] = "There were no assignments \nfor the given query";
                            TempData["Image"] = "/assets/icons/error.svg";
                            return Redirect("/nothing-found");
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

                    string urlCheckUser = _baseUrl + $"check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.GetAsync(urlCheckUser).Result;

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
                                string urlGetAssignment = _baseUrl + "assignment/" + assignmentId;
                                HttpResponseMessage getAssignmentRM = client.GetAsync(urlGetAssignment).Result;
                                
                                if(getAssignmentRM.IsSuccessStatusCode)
                                {
                                    Assignment assignment = getAssignmentRM.Content.ReadAsAsync<Assignment>().Result;
                                    if (assignment.IsActive)
                                    {
                                        ViewBag.Assignment = assignment;
                                        ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
                                        ViewBag.AcademicLevel = JsonConvert.DeserializeObject<List<string>>(client.GetAsync(_baseUrl + "academiclevel").Result.Content.ReadAsStringAsync().Result);
                                        ViewBag.Subject = JsonConvert.DeserializeObject<List<string>>(client.GetAsync(_baseUrl + "subject").Result.Content.ReadAsStringAsync().Result);
                                        string bitString = BitConverter.ToString(assignment.Timestamp);
                                        ViewBag.Timestamp  = bitString;
                                        return View("UpdateAssignment");
                                    }
                                    else
                                    {
                                        string urlCheckIfHasAcceptedSolution = _baseUrl + "assignment/check-if-has-accepted-solution/" + assignmentId;
                                        HttpResponseMessage CheckIfHasAcceptedSolutionRM = client.GetAsync(urlCheckIfHasAcceptedSolution).Result;
                                        if (CheckIfHasAcceptedSolutionRM
                                            .IsSuccessStatusCode)
                                        {
                                            return Redirect("/solution/solution-for-assignment/" + assignmentId);
                                        }
                                        else
                                        {
                                            return Redirect("/assignment/display-assignment/" + assignmentId);
                                        }
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

                        string urlCheckUser = _baseUrl + $"check-user-vs-assignment/{assignmentId}";
                        HttpResponseMessage returnCodeRM = client.GetAsync(urlCheckUser).Result;

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


                                    //assignment.AssignmentFile = Encoding.ASCII.GetBytes(collection["AssignmentFile"]);

                                    string urlUpdateAssignment = _baseUrl + "assignment/" + assignmentId;
                                    HttpResponseMessage updateAssignmentRM = client.PutAsync(urlUpdateAssignment,
                                        new StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")).Result;

                                    if (updateAssignmentRM.IsSuccessStatusCode)
                                    {
                                        TempData["Message"] = "Assignment updated successfully";
                                        TempData["ResponseStyleClass"] = "text-success";
                                        TempData["ButtonText"] = "Display your assignment";
                                        TempData["ButtonLink"] = "/assignment/display-assignment/" + assignmentId;
                                        TempData["PageTitle"] = "Assignment updated!";
                                        TempData["SubMessage"] = "Your assignment now waits for solvers to solve it";
                                        TempData["Image"] = "/assets/icons/success.svg";
                                        return Redirect("/success");
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
                    TempData["Message"] = "Assignment update failed";
                    TempData["ResponseStyleClass"] = "text-danger";
                    TempData["ButtonText"] = "Go back to the assignment form";
                    TempData["ButtonLink"] = "/assignment/update-assignment/" + assignmentId;
                    TempData["PageTitle"] = "Assignment update failed!";
                    TempData["SubMessage"] = "Invalid data inserted";
                    TempData["Image"] = "/assets/icons/error.svg";
                }
                return Redirect("/success");
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

                    string urlCheckUser = _baseUrl + $"check-user-vs-assignment/{assignmentId}";
                    HttpResponseMessage returnCodeRM = client.GetAsync(urlCheckUser).Result;

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
                                string urlMakeInactive = _baseUrl + "assignment/inactive/" + assignmentId;
                                HttpResponseMessage makeInactiveRM = client.PutAsync(urlMakeInactive, null).Result;

                                if (makeInactiveRM.IsSuccessStatusCode)
                                {
                                    TempData["Message"] = "Assignment deleted successfully";
                                    TempData["ResponseStyleClass"] = "text-success";
                                    TempData["ButtonText"] = "Go back to homepage";
                                    TempData["ButtonLink"] = "/";
                                    TempData["PageTitle"] = "Assignment deleted!";
                                    TempData["SubMessage"] = "Your assignment is now deleted";
                                    TempData["Image"] = "/assets/icons/success.svg";
                                    return Redirect("/success");
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

                    string urlGetAllAssignmentsForLoggedInUser = _baseUrl + "assignment/page-user/" + pageNumber;
                    HttpResponseMessage getAllAssignmentsForLoggedInUserRM = client.GetAsync(urlGetAllAssignmentsForLoggedInUser).Result;
                    
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
                        TempData["Message"] = "No assignment found";
                        TempData["ResponseStyleClass"] = "text-danger";
                        TempData["ButtonText"] = "Go back to homepage";
                        TempData["ButtonLink"] = "/";
                        TempData["PageTitle"] = "No assignments found!";
                        TempData["SubMessage"] = "There were no assignments \nfor the given query";
                        TempData["Image"] = "/assets/icons/error.svg";
                        return Redirect("/nothing-found");
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

                    string urlGetAllAssignmentsSolvedByLoggedInUser = _baseUrl + "assignment/solved-by-user";
                    HttpResponseMessage getAllAssignmentsSolvedByLoggedInUserRM = client.GetAsync(urlGetAllAssignmentsSolvedByLoggedInUser).Result;

                    if (getAllAssignmentsSolvedByLoggedInUserRM.IsSuccessStatusCode)
                    {
                        List<Assignment> solvedAssignments = getAllAssignmentsSolvedByLoggedInUserRM.Content.ReadAsAsync<List<Assignment>>().Result;

                        ViewBag.Assignments = solvedAssignments;
                        return View("AllAssignments");
                    }
                    else
                    {
                        TempData["Message"] = "No solutions found";
                        TempData["ResponseStyleClass"] = "text-danger";
                        TempData["ButtonText"] = "Go back to homepage";
                        TempData["ButtonLink"] = "/";
                        TempData["PageTitle"] = "No solutions found!";
                        TempData["SubMessage"] = "You have not solved \nany assignments yet!";
                        TempData["Image"] = "/assets/icons/error.svg";
                        return Redirect("/nothing-found");
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

                    string urlGetFileForDownload = _baseUrl + $"assignment/get-file/{assignmentId}";
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
