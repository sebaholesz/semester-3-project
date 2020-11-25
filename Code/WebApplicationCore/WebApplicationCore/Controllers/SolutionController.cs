using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using WebApplicationCore.Models;

namespace WebApplicationCore.Controllers
{
    public class SolutionController : Controller
    {

        [Route("solution/assignment/{id}")]
        [HttpGet]
        public ActionResult CreateSolution(int id)
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

                    var payload = new Dictionary<string, string>
                    {
                        {"AssignmentId", collection["SolutionModels.AssignmentId"]},
                        { "UserId", "12"},
                        {"Description", collection["SolutionModels.Description"]},
                        {"Timestamp", DateTime.Now.ToString()},
                        {"SolutionRating", "3.6M"},
                        {"Anonymous", collection["SolutionModels.Anonymous"][0]},
                    };

                    string strPayload = JsonConvert.SerializeObject(payload);
                    HttpContent httpContent = new StringContent(strPayload, Encoding.UTF8, "application/json");

                    string apiUrl = "https://localhost:44383/api/solution";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage message = client.PostAsync(apiUrl, httpContent).Result;
                        var responseContent = message.Content.ReadAsStringAsync().Result;
                        var responseContentTrimmed = responseContent.Trim('\"');
                        ViewBag.QueueOrder = responseContentTrimmed;
                        ViewBag.Message = message.StatusCode == HttpStatusCode.Created ? "Solution created successfully" : message.StatusCode == HttpStatusCode.Conflict ? "Ups. Solution creation failed!" : "";
                        ViewBag.ResponseStyleClass = message.StatusCode == HttpStatusCode.Created ? "text-success" : message.StatusCode == HttpStatusCode.Conflict ? "text-danger" : "";
                    }
                }
                else
                {
                    ViewBag.Message = "Insert correct data";
                    ViewBag.QueueOrder = -1;
                    ViewBag.ResponseStyleClass = "text-danger";
                }
                return View();
                return View("AfterPostingSolution");
            }
        }
    }
}
