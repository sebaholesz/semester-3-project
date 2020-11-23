using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApplicationCore.Models;

namespace WebApplicationCore.Controllers
{
    public class AssignmentController : Controller
    {
        [Route("assignment/create-assignment")]
        [HttpGet]
        public ActionResult CreateAssignment()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ViewBag.AcademicLevels = AssignmentModels.AcademicLevels();
                    ViewBag.Subjects = AssignmentModels.Subjects();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return View();
        }

        [Route("assignment/create-assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAssignmentAsync(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //it doesn't compile because we need to get the list from the API so we can check it
                    //string payloadAcademicLevel = "";
                    //if (AssignmentModels.AcademicLevelValues.Contains(collection["AcademicLevel"]))
                    //{
                    //    var i = AssignmentModels.AcademicLevelValues.IndexOf(collection["AcademicLevel"]);
                    //    payloadAcademicLevel = AssignmentModels.AcademicLevelValues[i];
                    //}

                    //string payloadSubject = "";
                    //if (AssignmentModels.SubjectValues.Contains(collection["Subject"]))
                    //{
                    //    var i = AssignmentModels.SubjectValues.IndexOf(collection["payloadSubject"]);
                    //    payloadSubject = AssignmentModels.SubjectValues[i];
                    //}

                    var payload = new Dictionary<string, string>
                    {
                        {"Title", collection["Title"] },
                        {"Description", collection["Description"]},
                        {"Price", collection["Price"]},
                        {"Deadline", collection["Deadline"]},
                        {"Anonymous", collection["Anonymous"][0]},
                        {"AcademicLevel", collection["AcademicLevel"]},
                        {"Subject", collection["Subject"]},
                    };

                    string strPayload = JsonConvert.SerializeObject(payload);
                    HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");

                    string u = "https://localhost:44383/api/assignment";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage message = client.PostAsync(u, c).Result;
                        var responseContent = await message.Content.ReadAsStringAsync();
                        var responseContentTrimmed = responseContent.Trim('\"');
                        ViewBag.AcademicLevels = AssignmentModels.AcademicLevels();
                        ViewBag.Subjects = AssignmentModels.Subjects();
                        ViewBag.Message = responseContentTrimmed;
                        ViewBag.ResponseStyleClass = message.StatusCode == HttpStatusCode.Created ? "text-success" : message.StatusCode == HttpStatusCode.NotFound ? "text-danger" : "";
                    }
                }
                else
                {
                    ViewBag.Message = "Insert correct data";
                    ViewBag.ResponseStyleClass = "text-danger";
                }
                return View("CreateAssignment");

            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                ViewBag.ResponseStyleClass = "text-danger";
                return View("CreateAssignment");
            }
        }

        [Route("assignment/display-assignment/{id}")]
        [HttpGet]
        public ActionResult DisplayAssignment(int id)
        {
            string url = "https://localhost:44383/api/assignment/" + id;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage message = client.GetAsync(url).Result;
                    string responseContent = message.Content.ReadAsStringAsync().Result;
                    AssignmentModels assignment = JsonConvert.DeserializeObject<AssignmentModels>(responseContent);
                    ViewBag.Assignment = assignment;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return View("DisplayAssignment");
        }

        [Route("assignment/solving-assignment/{id}")]
        [HttpGet]
        public ActionResult DisplaySolvingAssignment(int id)
        {
            string url = "https://localhost:44383/api/assignment/" + id;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage message = client.GetAsync(url).Result;
                    string responseContent = message.Content.ReadAsStringAsync().Result;
                    AssignmentModels assignment = JsonConvert.DeserializeObject<AssignmentModels>(responseContent);
                    ViewBag.Assignment = assignment;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return View("SolvingAssignment");
        }

        //public ActionResult AssignmentPartial()
        //{
        //    string url = "https://localhost:44383/api/assignment/7";
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage message = client.GetAsync(url).Result;
        //            string responseContent = message.Content.ReadAsStringAsync().Result;
        //            AssignmentModels assignment = JsonConvert.DeserializeObject<AssignmentModels>(responseContent);
        //            ViewBag.Assignment = assignment;
        //        }
        //        catch (Exception e)
        //        { 
        //            throw e;
        //        }
        //    }

        //    return PartialView("AssignmentPartial");
        //}
    }
}
