using BusinessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

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
            }
            catch (Exception e)
            {
                throw e;
            }
            return View("CreateSolution");
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
                        ViewBag.QueueOrder = queueOrder;
                        ViewBag.Message = "Solution created successfully";
                        ViewBag.ResponseStyleClass ="text-success";
                    }
                    else
                    {
                        ViewBag.Message = "Ups. Solution creation failed!";
                        ViewBag.ResponseStyleClass = "text-danger";
                    }
                }
                else
                {
                    ViewBag.Message = "Insert correct data";
                    ViewBag.QueueOrder = -1;
                    ViewBag.ResponseStyleClass = "text-danger";
                }
                return View("AfterPostingSolution");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }
    }
}
