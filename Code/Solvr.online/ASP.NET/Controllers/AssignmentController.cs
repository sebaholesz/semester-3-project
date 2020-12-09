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
using System.Text;
using System.Threading.Tasks;

namespace webApi.Controllers
{
   // [Authorize]
    public class AssignmentController : Controller
    {
    

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
                    string urlGetAllAcademicLevels = "https://localhost:44316/apiV1/academiclevel";
                    string urlGetAllSubjects = "https://localhost:44316/apiV1/subject";
                    ViewBag.AcademicLevels = JsonConvert.DeserializeObject<List<string>>((client.GetAsync(urlGetAllAcademicLevels).Result).Content.ReadAsStringAsync().Result);
                    ViewBag.Subjects = JsonConvert.DeserializeObject<List<string>>( (client.GetAsync(urlGetAllSubjects).Result).Content.ReadAsStringAsync().Result);
                    return View("CreateAssignment");
                }
            }
            catch (Exception e)
            {
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
                        Assignment assignment = new Assignment();
                        assignment.Title = collection["Title"];
                        assignment.Description = collection["Description"];
                        assignment.Price = Convert.ToInt32(collection["Price"]);
                        assignment.Deadline = Convert.ToDateTime(collection["Deadline"]);
                        assignment.Anonymous = Convert.ToBoolean(collection["Anonymous"][0]);
                        assignment.AcademicLevel = collection["AcademicLevel"];
                        assignment.Subject = collection["Subject"];
                        assignment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (files != null)
                        {
                            var dataStream = new MemoryStream();
                            await files.CopyToAsync(dataStream);
                            assignment.AssignmentFile = dataStream.ToArray();
                            dataStream.Close();
                        }

                        string urlCreateAssignment = "https://localhost:44316/apiV1/assignment";
                        int returnCode = (client.PostAsync(
                            urlCreateAssignment,
                            new StringContent(JsonConvert.SerializeObject(assignment), Encoding.UTF8, "application/json")
                            ).Result).Content.ReadAsAsync<int>().Result;

                        if (returnCode >= 1)
                        {
                            ViewBag.Message = "Assignment created successfully";
                            ViewBag.ResponseStyleClass = "text-success";
                            ViewBag.ButtonText = "Display your assignment";
                            ViewBag.ButtonLink = "/assignment/display-assignment/" + returnCode;
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
        [Route("assignment/display-assignment/{assignmentId}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
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
                            string urlCompleteAssignmentData = "https://www.localhost:44316/apiV1/assignment/complete-data/" + assignmentId;
                            AssignmentSolutionUser asu = JsonConvert.DeserializeObject<AssignmentSolutionUser>((client.GetAsync(urlCompleteAssignmentData).Result).Content.ReadAsStringAsync().Result);
                            ViewBag.Assignment = asu.Assingment;
                            string urlCountOfSolutions = "solution/CountByAssignmentId/{assignmentId}";
                            ViewBag.SolutionCount = JsonConvert.DeserializeObject<int>((client.GetAsync(urlCountOfSolutions).Result).Content.ReadAsStringAsync().Result);
                            ViewBag.User = asu.User;
                            return View("DisplayAssignment");
                        case 1:
                            return Redirect("/assignment/update-assignment/" + assignmentId);
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

        //can be accessed by everybody
        [AllowAnonymous]
        [Route("assignment/display-assignments")]
        [HttpGet]
        public ActionResult DisplayAllAssignments()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        throw new NotImplementedException("we need to get all active not solved by this user");
                    }
                    else
                    {
                        throw new NotImplementedException("we need to get all active");

                        string urlGetAllAssignments = "https://localhost:44316/apiV1/assignment";
                        List<Assignment> assignments = client.GetAsync(urlGetAllAssignments).Result.Content.ReadAsAsync<List<Assignment>>().Result;

                        if (assignments.Count > 0)
                        {
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
            }
            catch (Exception e)
            {
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
                            string urlGetAssignment = "https://www.localhost:44316/apiV1/assignment/" + assignmentId;
                            Assignment assignment = JsonConvert.DeserializeObject<Assignment>((client.GetAsync(urlGetAssignment).Result).Content.ReadAsStringAsync().Result);
                            if (assignment.IsActive)
                            {
                                ViewBag.Assignment = assignment;
                                ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
                                return View("UpdateAssignment");
                            }
                            else
                            {
                                return Redirect("/solution/solution-for-assignment/" + assignmentId);
                            }
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
                                string urlGetAssignment = "https://www.localhost:44316/apiV1/assignment/" + assignmentId;
                                Assignment assignment = JsonConvert.DeserializeObject<Assignment>((client.GetAsync(urlGetAssignment).Result).Content.ReadAsStringAsync().Result);
                                if (assignment.IsActive)
                                {
                                    assignment.Title = collection["Title"];
                                    assignment.Description = collection["Description"];
                                    assignment.Price = Convert.ToInt32(collection["Price"]);
                                    //maybe the parse not needed?
                                    assignment.Deadline = DateTime.Parse(collection["Deadline"]);
                                    assignment.Anonymous = Convert.ToBoolean(collection["Anonymous"][0]);
                                    assignment.AcademicLevel = collection["AcademicLevel"];
                                    assignment.Subject = collection["Subject"];

                                    //TODO check if the file should be updated
                                    //assignment.AssignmentFile = Encoding.ASCII.GetBytes(collection["AssignmentFile"]);

                                    string urlUpdateAssignment = "https://www.localhost:44316/apiV1/assignment/" + assignmentId;
                                    int noOfRowsAffected = JsonConvert.DeserializeObject<int>((client.PutAsync(urlUpdateAssignment, new StringContent(JsonConvert.SerializeObject(assignment))).Result).Content.ReadAsStringAsync().Result);

                                    //TODO notify all solvers of the changes
                                    if (noOfRowsAffected > 0)
                                    {
                                        ViewBag.Message = "Assignment updated successfully";
                                        ViewBag.ResponseStyleClass = "text-success";
                                        ViewBag.ButtonText = "Display your assignment";
                                        ViewBag.ButtonLink = "/assignment/display-assignment/" + assignmentId;
                                        ViewBag.PageTitle = "Assignment updated!";
                                        ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                                        ViewBag.Image = "/assets/icons/success.svg";
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Assignment update failed";
                                        ViewBag.ResponseStyleClass = "text-danger";
                                        ViewBag.ButtonText = "Go back to the assignment form";
                                        ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
                                        ViewBag.PageTitle = "Assignment update failed!";
                                        ViewBag.SubMessage = "There was a server error \ntry again later";
                                        ViewBag.Image = "/assets/icons/error.svg";
                                    }
                                    return View("UserFeedback");
                                }
                                else
                                {
                                    throw new Exception("Cannot update inactive assignment");
                                }
                            case 2:
                                return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
                            default:
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
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        ///*can be accessed by everybody who 
        // * posted the assignment
        // * and only if the assignment is still active
        // */
        [Route("assignment/delete-assignment/{assignmentId}")]
        [HttpDelete]
        public ActionResult DeleteAssignment(int assignmentId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
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
                            string urlMakeInactive = "https://www.localhost:44316/apiV1/assignment/inactive" + assignmentId;
                            int noOfRowsAffected = Convert.ToInt32((client.PutAsync(urlMakeInactive, null).Result).Content.ReadAsStringAsync().Result);

                            if (noOfRowsAffected == 1)
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
                                ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
                                ViewBag.PageTitle = "Assignment deletion failed!";
                                ViewBag.SubMessage = "There was a server error \ntry again later";
                                ViewBag.Image = "/assets/icons/error.svg";
                            }
                            return View("UserFeedback");
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
         * posted the assignment
         */
        [Route("assignment/user")]
        [HttpGet]
        public ActionResult GetAllAssignmentsForLoggedInUser()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    throw new NotImplementedException("we need to get all for user");

                    string urlGetAllAssignments = "https://localhost:44316/apiV1/assignment";
                    List<Assignment> assignments = client.GetAsync(urlGetAllAssignments).Result.Content.ReadAsAsync<List<Assignment>>().Result;

                    if (assignments.Count > 0)
                    {
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
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    throw new NotImplementedException("we need to get all solved by user");

                    string urlGetAllAssignments = "https://localhost:44316/apiV1/assignment";
                    List<Assignment> solvedAssignments = client.GetAsync(urlGetAllAssignments).Result.Content.ReadAsAsync<List<Assignment>>().Result;

                    if (solvedAssignments.Count > 0)
                    {
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
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }
    }
}
