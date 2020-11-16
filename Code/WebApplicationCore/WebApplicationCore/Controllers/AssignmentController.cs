using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplicationCore.Models;

namespace WebApplicationCore.Controllers
{
    public class AssignmentController : Controller
    {

        [Route("assignment/create-assignment")]
        [HttpGet]
        public ActionResult CreateAssignment()
        {
            //modify the URL so it gets the list off academic levels and subjects
            string u = "https://localhost:44383/api/";

            using (HttpClient client = new HttpClient())
            {
                //get the list off academic levels and subjects
                HttpResponseMessage message = client.GetAsync(u).Result;
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
                    string payloadAcademicLevel = "";
                    if (AssignmentModels.AcademicLevelValues.Contains(collection["AcademicLevel"]))
                    {
                        var i = AssignmentModels.AcademicLevelValues.IndexOf(collection["AcademicLevel"]);
                        payloadAcademicLevel = AssignmentModels.AcademicLevelValues[i];
                    }

                    string payloadSubject = "";
                    if (AssignmentModels.SubjectValues.Contains(collection["Subject"]))
                    {
                        var i = AssignmentModels.SubjectValues.IndexOf(collection["payloadSubject"]);
                        payloadSubject = AssignmentModels.SubjectValues[i];
                    }

                    var payload = new Dictionary<string, string>
                    {
                        {"Title", collection["Title"] },
                        {"Description", collection["Description"]},
                        {"Price", collection["Price"]},
                        {"Deadline", collection["Deadline"]},
                        {"Anonymous", collection["Anonymous"]},
                        {"AcademicLevel", payloadAcademicLevel},
                        {"Subject", payloadSubject},
                    };

                    string strPayload = JsonConvert.SerializeObject(payload);
                    HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");

                    string u = "https://localhost:44383/api/assignment";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage message = client.PostAsync(u, c).Result;
                        var responseContent = await message.Content.ReadAsStringAsync();
                        var responseContentTrimmed = responseContent.Trim('\"');
                        ViewBag.Message = responseContentTrimmed;
                        ViewBag.ResponseStyleClass = message.StatusCode == HttpStatusCode.Created ? "text-success" : message.StatusCode == HttpStatusCode.NotFound ? "text-danger" : "";
                    }
                }
                ViewBag.Message = "Insert correct data";
                ViewBag.ResponseStyleClass = "text-danger";
                return View("CreateAssignment");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                ViewBag.ResponseStyleClass = "text-danger";
                return View("CreateAssignment");
            }
        }
    }
}
