﻿using ASP.NET.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeDetective;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace webApi.Controllers
{
    [Authorize]
    public class SolutionController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly string _baseUrl;

        public SolutionController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _baseUrl = "https://localhost:44316/apiV1/";
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
                                HttpResponseMessage getCompleteAssignmentDataRM = client.GetAsync(urlCompleteAssignmentData).Result;
                                
                                if(getCompleteAssignmentDataRM.IsSuccessStatusCode)
                                {
                                    AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
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
                                    return View("CreateSolution");
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
                                    Solution solution = new Solution();
                                    solution.AssignmentId = assignmentId;
                                    solution.Description = collection["Solution.Description"];
                                    solution.Timestamp = DateTime.UtcNow;
                                    solution.Anonymous = Convert.ToBoolean(collection["Solution.Anonymous"][0]);
                                    solution.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                                    if (files != null)
                                    {
                                        var dataStream = new MemoryStream();
                                        await files.CopyToAsync(dataStream);
                                        solution.SolutionFile = dataStream.ToArray();
                                        dataStream.Close();
                                    }


                                    string urlCreateSolution = _baseUrl + "solution/";
                                    HttpResponseMessage createSolutionRM = client.PostAsync(urlCreateSolution, new StringContent(JsonConvert.SerializeObject(solution), Encoding.UTF8, "application/json")).Result;
                                    if(createSolutionRM.IsSuccessStatusCode)
                                    {
                                        int queueOrder = createSolutionRM.Content.ReadAsAsync<int>().Result;

                                        TempData["Message"] = "Solution created successfully";
                                        TempData["ResponseStyleClass"] = "text-success";
                                        TempData["ButtonText"] = "Go back to homepage";
                                        TempData["ButtonLink"] = "/";
                                        TempData["PageTitle"] = "Solution created!";
                                        TempData["SubMessage"] = "You are number " + queueOrder + " in the queue";
                                        TempData["Image"] = "/assets/icons/success.svg";
                                        return Redirect("/success");
                                    }
                                    else
                                    {
                                        throw new Exception(createSolutionRM.Content.ReadAsStringAsync().Result);
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
                else
                {
                    throw new Exception("Solution creation failed");
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
         * and is logged in
         */
        [Route("solution/solution-queue/{assignmentId}")]
        [HttpGet]
        public ActionResult ChooseSolutionGet(int assignmentId)
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

                                string urlGetAllSolutionsForAssignment = _baseUrl + $"solution/by-assignment/{assignmentId}";
                                HttpResponseMessage getAllSolutionsForAssignmentRM = client.GetAsync(urlGetAllSolutionsForAssignment).Result;
                               

                                if (getAllSolutionsForAssignmentRM.IsSuccessStatusCode)
                                {
                                    ViewBag.Solutions = getAllSolutionsForAssignmentRM.Content.ReadAsAsync<List<Solution>>().Result;
                                    return View("DisplayAllSolutionsForAssignment");
                                }
                                else
                                {
                                    TempData["Message"] = "No solutions found";
                                    TempData["ResponseStyleClass"] = "text-danger";
                                    TempData["ButtonText"] = "Go back to the assignment";
                                    TempData["ButtonLink"] = "/assignment/display-assignment/" + assignmentId;
                                    TempData["PageTitle"] = "No solutions found!";
                                    TempData["SubMessage"] = "No one has solved your assignment yet";
                                    TempData["Image"] = "/assets/icons/error.svg";
                                    return Redirect("/nothing-found");
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
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string[] reqBodyStringArray = reqBody.Split("*");
                    int solutionId = Convert.ToInt32(reqBodyStringArray[0]);
                    int assignmentId = Convert.ToInt32(reqBodyStringArray[1]);


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
                                string urlChooseSolution = _baseUrl + $"solution/choose-solution";

                                List<int> ids = new List<int>() { solutionId, assignmentId };

                                HttpResponseMessage chooseSolutionRM = client.PostAsync(urlChooseSolution, new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json")).Result;


                                if (chooseSolutionRM.IsSuccessStatusCode)
                                {
                                    string urlCompleteAssignmentDataWithSolution = _baseUrl + "assignment/complete-data-with-accepted-solution/" + assignmentId;
                                    HttpResponseMessage getCompleteAssignmentDataWithSolutionRM = client.GetAsync(urlCompleteAssignmentDataWithSolution).Result;

                                    if (getCompleteAssignmentDataWithSolutionRM.IsSuccessStatusCode)
                                    {
                                        AssignmentSolutionUser asu = getCompleteAssignmentDataWithSolutionRM.Content.ReadAsAsync<AssignmentSolutionUser>().Result;
                                        ViewBag.Assignment = asu.Assignment;
                                        ViewBag.Solution = asu.Solution;
                                        ViewBag.User = asu.User;

                                        return View("DisplaySolution");
                                    }
                                    else
                                    {
                                        throw new Exception(getCompleteAssignmentDataWithSolutionRM.Content.ReadAsStringAsync().Result);
                                    }
                                }
                                else
                                {
                                    throw new Exception(chooseSolutionRM.ReasonPhrase);
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
                                string urlGetCompleteAssignmentData = _baseUrl + "assignment/complete-data-with-accepted-solution/" + assignmentId;
                                HttpResponseMessage getCompleteAssignmentDataRM = client.GetAsync(urlGetCompleteAssignmentData).Result;
                               
                                if(getCompleteAssignmentDataRM.IsSuccessStatusCode)
                                {
                                    AssignmentSolutionUser asu = getCompleteAssignmentDataRM.Content.ReadAsAsync<AssignmentSolutionUser>().Result;
                                    ViewBag.Assignment = asu.Assignment;
                                    ViewBag.Solution = asu.Solution;
                                    ViewBag.User = asu.User;

                                    return View("DisplaySolution");
                                }
                                else
                                {
                                    throw new Exception(getCompleteAssignmentDataRM.ReasonPhrase);
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
                                return Redirect("/assignment/display-assignment/" + assignmentId);
                            case 2:
                                string urlGetCompleteAssignmentData = _baseUrl + $"assignment/complete-data-with-solution/{assignmentId}";
                                HttpResponseMessage getCompleteAssignmentDataRM = client.GetAsync(urlGetCompleteAssignmentData).Result;

                                if (getCompleteAssignmentDataRM.IsSuccessStatusCode)
                                {
                                    AssignmentSolutionUser asu = getCompleteAssignmentDataRM.Content.ReadAsAsync<AssignmentSolutionUser>().Result;
                                    ViewBag.Assignment = asu.Assignment;
                                    ViewBag.Solution = asu.Solution;
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

                                    return View("DisplaySolution");
                                }
                                else
                                {
                                    throw new Exception(getCompleteAssignmentDataRM.ReasonPhrase);
                                }
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
         * is logged in
         */
        [Route("solution/download-file/{solutionId}")]
        [HttpGet]
        public ActionResult DownloadFile(int solutionId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetFileForDownload = _baseUrl + $"solution/get-file/{solutionId}";
                    HttpResponseMessage getFileForDownloadRM = client.GetAsync(urlGetFileForDownload).Result;

                    if (getFileForDownloadRM.IsSuccessStatusCode)
                    {
                        byte[] fileContent = getFileForDownloadRM.Content.ReadAsAsync<byte[]>().Result;
                        string extension = fileContent.GetFileType().Extension;
                        string fileName = $"{solutionId}-solution-file_{DateTime.Today.Day}-{DateTime.Today.Month}-{DateTime.Today.Year}.{extension}";
                        return File(fileContent, "application/force-download", fileName);
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
