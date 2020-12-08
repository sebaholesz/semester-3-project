using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    

        public AssignmentController()
        {
   
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
                //ViewBag.AcademicLevels = assignmentBusiness.GetAllAcademicLevels();
                //ViewBag.Subjects = assignmentBusiness.GetAllSubjects();
                return View("CreateAssignment");
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
                    //int assignmentId = assignmentBusiness.CreateAssignment(assignment);

                //    if (assignmentId >= 1)
                //    {
                //        ViewBag.Message = "Assignment created successfully";
                //        ViewBag.ResponseStyleClass = "text-success";
                //        ViewBag.ButtonText = "Display your assignment";
                //        ViewBag.ButtonLink = "/assignment/display-assignment/" + assignmentId;
                //        ViewBag.PageTitle = "Assignment created!";
                //        ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
                //        ViewBag.Image = "/assets/icons/success.svg";
                //    }
                //    else
                //    {
                //        ViewBag.Message = "Assignment creation failed";
                //        ViewBag.ResponseStyleClass = "text-danger";
                //        ViewBag.ButtonText = "Go back to the assignment form";
                //        ViewBag.ButtonLink = "/assignment/create-assignment/";
                //        ViewBag.PageTitle = "Assignment creation failed!";
                //        ViewBag.SubMessage = "There was a server error \ntry again later";
                //        ViewBag.Image = "/assets/icons/error.svg";
                //    }
                //}
                //else
                //{
                //    ViewBag.Message = "Assignment creation failed";
                //    ViewBag.ResponseStyleClass = "text-danger";
                //    ViewBag.ButtonText = "Go back to the assignment form";
                //    ViewBag.ButtonLink = "/assignment/create-assignment/";
                //    ViewBag.PageTitle = "Assignment creation failed!";
                //    ViewBag.SubMessage = "Invalid data inserted";
                //    ViewBag.Image = "/assets/icons/error.svg";
                }
                return View("UserFeedback");
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
        //[Route("assignment/display-assignment/{assignmentId}")]
        //[HttpGet]
        //public ActionResult DisplayAssignment(int assignmentId)
        //{
        //    try
        //    {
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        //int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

        //        /*
        //         * 0 = hes neither author nor previous solver
        //         * 1 = authorUserId = currentUserId
        //         * 2 = solverId = currentUserId
        //         */
        //        //switch (returnCode)
        //        //{
        //        //    case 0:
        //        //        //Assignment assignment = assignmentBusiness.GetByAssignmentId(assignmentId);
        //        //        //ViewBag.Assignment = assignment;
        //        //        //ViewBag.Solutions = solutionBusiness.GetSolutionsByAssignmentId(assignmentId).Count;
        //        //        //ViewBag.Username = userBusiness.GetUserUsername(assignment.UserId);
        //        //        //if (assignment.Anonymous)
        //        //        {
        //        //            ViewBag.Name = "";
        //        //        }
        //        //        else
        //        //        {
        //        //            //ViewBag.Name = userBusiness.GetUserName(assignment.UserId);
        //        //        }
        //        //        return View("DisplayAssignment");
        //        //    case 1:
        //        //        return Redirect("/assignment/update-assignment/" + assignmentId);
        //        //    case 2:
        //        //        return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
        //        //    default:
        //        //        throw new Exception("Internal server error");
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ////can be accessed by everybody
        //[AllowAnonymous]
        //[Route("assignment/display-assignments")]
        //[HttpGet]
        //public ActionResult DisplayAllAssignments()
        //{
        //    try
        //    {
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignmentsNotSolvedByUser(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //            if (assignments.Count > 0)
        //            {
        //                ViewBag.Assignments = assignments;
        //                return View("AllAssignments");
        //            }
        //            else
        //            {
        //                ViewBag.Message = "No assignment found";
        //                ViewBag.ResponseStyleClass = "text-danger";
        //                ViewBag.ButtonText = "Go back to homepage";
        //                ViewBag.ButtonLink = "/";
        //                ViewBag.PageTitle = "No assignments found!";
        //                ViewBag.SubMessage = "There were no assignments \nfor the given query";
        //                ViewBag.Image = "/assets/icons/error.svg";
        //                return View("UserFeedback");
        //            }
        //        }
        //        else 
        //        {
        //            List<Assignment> assignments = assignmentBusiness.GetAllActiveAssignments();
        //            if (assignments.Count > 0)
        //            {
        //                ViewBag.Assignments = assignments;
        //                return View("AllAssignments");
        //            }
        //            else
        //            {
        //                ViewBag.Message = "No assignment found";
        //                ViewBag.ResponseStyleClass = "text-danger";
        //                ViewBag.ButtonText = "Go back to homepage";
        //                ViewBag.ButtonLink = "/";
        //                ViewBag.PageTitle = "No assignments found!";
        //                ViewBag.SubMessage = "There were no assignments \nfor the given query";
        //                ViewBag.Image = "/assets/icons/error.svg";
        //                return View("UserFeedback");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ///*can be accessed by everybody who 
        // * posted the assignment
        // * and only if the assignment is still active
        // */
        //[Route("assignment/update-assignment/{assignmentId}")]
        //[HttpGet]
        //public ActionResult UpdateAssignment(int assignmentId)
        //{
        //    try
        //    {
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

        //        /*
        //         * 0 = hes neither author nor previous solver
        //         * 1 = authorUserId = currentUserId
        //         * 2 = solverId = currentUserId
        //         */
        //        switch (returnCode)
        //        {
        //            case 0:
        //                return Redirect("/assignment/display-assignment/" + assignmentId);
        //            case 1:
        //                Assignment assignment = assignmentBusiness.GetByAssignmentId(assignmentId);
        //                if (assignment.IsActive)
        //                {
        //                    ViewBag.Assignment = assignment;
        //                    ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
        //                    return View("UpdateAssignment");
        //                }
        //                else
        //                {
        //                    ViewBag.Assignment = assignment;
        //                    ViewBag.AssignmentDeadline = assignment.Deadline.ToString("yyyy-MM-ddTHH:mm:ss");
        //                    return Redirect("/solution/solution-for-assignment/" + assignmentId);
        //                }
        //            case 2:
        //                return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
        //            default:
        //                throw new Exception("Internal server error");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ///*can be accessed by everybody who 
        // * posted the assignment
        // * and only if the assignment is still active
        // */
        //[Route("assignment/update-assignment/{assignmentId}")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateAssignment(IFormCollection collection, int assignmentId)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //            int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

        //            /*
        //             * 0 = hes neither author nor previous solver
        //             * 1 = authorUserId = currentUserId
        //             * 2 = solverId = currentUserId
        //             */
        //            switch (returnCode)
        //            {
        //                case 0:
        //                    return Redirect("/assignment/display-assignment/" + assignmentId);
        //                case 1:
        //                    Assignment assignment = assignmentBusiness.GetByAssignmentId(assignmentId);
        //                    if (assignment.IsActive)
        //                    {
        //                        assignment.Title = collection["Title"];
        //                        assignment.Description = collection["Description"];
        //                        assignment.Price = Convert.ToInt32(collection["Price"]);
        //                        //maybe the parse not needed?
        //                        assignment.Deadline = DateTime.Parse(collection["Deadline"]);
        //                        assignment.Anonymous = Convert.ToBoolean(collection["Anonymous"][0]);
        //                        assignment.AcademicLevel = collection["AcademicLevel"];
        //                        assignment.Subject = collection["Subject"];

        //                        //TODO check if the file should be updated
        //                        //assignment.AssignmentFile = Encoding.ASCII.GetBytes(collection["AssignmentFile"]);

        //                        int noOfRowsAffected = assignmentBusiness.UpdateAssignment(assignment, assignmentId);

        //                        //TODO notify all solvers of the changes
        //                        if (noOfRowsAffected > 0)
        //                        {
        //                            ViewBag.Message = "Assignment updated successfully";
        //                            ViewBag.ResponseStyleClass = "text-success";
        //                            ViewBag.ButtonText = "Display your assignment";
        //                            ViewBag.ButtonLink = "/assignment/display-assignment/" + assignmentId;
        //                            ViewBag.PageTitle = "Assignment updated!";
        //                            ViewBag.SubMessage = "Your assignment now waits for solvers to solve it";
        //                            ViewBag.Image = "/assets/icons/success.svg";
        //                        }
        //                        else
        //                        {
        //                            ViewBag.Message = "Assignment update failed";
        //                            ViewBag.ResponseStyleClass = "text-danger";
        //                            ViewBag.ButtonText = "Go back to the assignment form";
        //                            ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
        //                            ViewBag.PageTitle = "Assignment update failed!";
        //                            ViewBag.SubMessage = "There was a server error \ntry again later";
        //                            ViewBag.Image = "/assets/icons/error.svg";
        //                        }
        //                        return View("UserFeedback");
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("Cannot update inactive assignment");
        //                    }
        //                case 2:
        //                    return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
        //                default:
        //                    throw new Exception("Internal server error");
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Message = "Assignment update failed";
        //            ViewBag.ResponseStyleClass = "text-danger";
        //            ViewBag.ButtonText = "Go back to the assignment form";
        //            ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
        //            ViewBag.PageTitle = "Assignment update failed!";
        //            ViewBag.SubMessage = "Invalid data inserted";
        //            ViewBag.Image = "/assets/icons/error.svg";
        //        }
        //        return View("UserFeedback");
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ///*can be accessed by everybody who 
        // * posted the assignment
        // * and only if the assignment is still active
        // */
        //[Route("assignment/delete-assignment/{assignmentId}")]
        //[HttpDelete]
        //public ActionResult DeleteAssignment(int assignmentId)
        //{
        //    try
        //    {
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        int returnCode = assignmentBusiness.CheckUserVsAssignment(assignmentId, userId);

        //        /*
        //         * 0 = hes neither author nor previous solver
        //         * 1 = authorUserId = currentUserId
        //         * 2 = solverId = currentUserId
        //         */
        //        switch (returnCode)
        //        {
        //            case 0:
        //                return Redirect("/assignment/display-assignment/" + assignmentId);
        //            case 1:
        //                Assignment assignment = assignmentBusiness.GetByAssignmentId(assignmentId);
        //                if (assignment.IsActive)
        //                {
        //                    int noOfRowsAffected = assignmentBusiness.MakeAssignmentInactive(assignmentId);

        //                    if (noOfRowsAffected > 0)
        //                    {
        //                        ViewBag.Message = "Assignment deleted successfully";
        //                        ViewBag.ResponseStyleClass = "text-success";
        //                        ViewBag.ButtonText = "Go back to homepage";
        //                        ViewBag.ButtonLink = "/";
        //                        ViewBag.PageTitle = "Assignment deleted!";
        //                        ViewBag.SubMessage = "Your assignment is now deleted";
        //                        ViewBag.Image = "/assets/icons/success.svg";
        //                    }
        //                    else
        //                    {
        //                        ViewBag.Message = "Assignment deletion failed";
        //                        ViewBag.ResponseStyleClass = "text-danger";
        //                        ViewBag.ButtonText = "Go back to the assignment form";
        //                        ViewBag.ButtonLink = "/assignment/update-assignment/" + assignmentId;
        //                        ViewBag.PageTitle = "Assignment deletion failed!";
        //                        ViewBag.SubMessage = "There was a server error \ntry again later";
        //                        ViewBag.Image = "/assets/icons/error.svg";
        //                    }
        //                    return View("UserFeedback");
        //                }
        //                else
        //                {
        //                    throw new Exception("Cannot delete inactive assignment");
        //                }
        //            case 2:
        //                return Redirect("/solution/my-solution-for-assignment/" + assignmentId);
        //            default:
        //                throw new Exception("Internal server error");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ///*can be accessed by everybody who 
        // * posted the assignment
        // */
        //[Route("assignment/user")]
        //[HttpGet]
        //public ActionResult GetAllAssignmentsForLoggedInUser()
        //{
        //    try
        //    {
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        List<Assignment> assignments = assignmentBusiness.GetAllAssignmentsForUser(userId);

        //        if (assignments.Count > 0)
        //        {
        //            ViewBag.Assignments = assignments;
        //            return View("AllAssignments");
        //        }
        //        else
        //        {
        //            ViewBag.Message = "No assignment found";
        //            ViewBag.ResponseStyleClass = "text-danger";
        //            ViewBag.ButtonText = "Go back to homepage";
        //            ViewBag.ButtonLink = "/";
        //            ViewBag.PageTitle = "No assignments found!";
        //            ViewBag.SubMessage = "There were no assignments \nfor the given query";
        //            ViewBag.Image = "/assets/icons/error.svg";
        //            return View("UserFeedback");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}

        ///*can be accessed by everybody who 
        // * solved the assignment
        // */
        //[Route("assignment/solved-by-user")]
        //[HttpGet]
        //public ActionResult GetAllAssignmentsSolvedByLoggedInUser()
        //{
        //    try
        //    {
        //        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        List<Assignment> solvedAssignments = assignmentBusiness.GetAllAssignmentsSolvedByUser(userId);

        //        if (solvedAssignments.Count > 0)
        //        {
        //            ViewBag.Assignments = solvedAssignments;
        //            return View("AllAssignments");
        //        }
        //        else
        //        {
        //            ViewBag.Message = "No solutions found";
        //            ViewBag.ResponseStyleClass = "text-danger";
        //            ViewBag.ButtonText = "Go back to homepage";
        //            ViewBag.ButtonLink = "/";
        //            ViewBag.PageTitle = "No soluitons found!";
        //            ViewBag.SubMessage = "You have not solved \nany assignments yet!";
        //            ViewBag.Image = "/assets/icons/error.svg";
        //            return View("UserFeedback");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        TempData["ErrorMessage"] = e.Message;
        //        return Redirect("/error");
        //    }
        //}
    }
}
