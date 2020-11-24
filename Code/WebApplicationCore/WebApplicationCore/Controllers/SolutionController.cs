using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
                        {"AssignmentId", collection["AssignmentId"]},
                        { "UserId", "12"},
                        {"Description", collection["Description"]},
                        {"Timestamp", DateTime.Now.ToString()},
                        {"SolutionRating", "3.6M"},
                        {"Anonymous", collection["Anonymous"][0]},
                    };

                    string strPayload = JsonConvert.SerializeObject(payload);
                    HttpContent httpContent = new StringContent(strPayload, Encoding.UTF8, "application/json");

                    string apiUrl = "https://localhost:44383/api/solution";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage message = client.PostAsync(apiUrl, httpContent).Result;
                        var responseContent = message.Content.ReadAsStringAsync().Result;
                        var responseContentTrimmed = responseContent.Trim('\"');
                        ViewBag.Message = responseContentTrimmed;
                        ViewBag.ResponseStyleClass = message.StatusCode == HttpStatusCode.Created ? "text-success" : message.StatusCode == HttpStatusCode.NotFound ? "text-danger" : "";
                    }
                }
                else
                {
                    ViewBag.Message = "Insert correct data";
                    ViewBag.ResponseStyleClass = "text-danger";
                }
                return View("CreateSolution");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                ViewBag.ResponseStyleClass = "text-danger";
                return View("CreateSolution");
            }
        }
    }
}
