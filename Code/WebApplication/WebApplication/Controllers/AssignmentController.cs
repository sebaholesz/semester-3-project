using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AssignmentController : Controller
    {

        [Route("assignment/create-assignment")]
        [HttpGet]
        public ActionResult CreateAssignment()
        {
            return View();
        }

        [Route("assignment/create-assignment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAssignmentAsync(FormCollection collection)
            {
            try
            {
                if (ModelState.IsValid)
                {
                    var payload = new Dictionary<string, string>
                    {
                        {"Description", collection["assignmentDescription"]},
                        {"Price", collection["assignmentCredits"]},
                        {"Deadline", collection["assignmentDeadline"]},
                        {"Anonymous", collection["assignmentIsAnonymous"]}
                        //missing some of the values both from model and from the view
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
                        ViewBag.StatusCode = message.StatusCode;
                    }
                }
                return View("CreateAssignment");
            }
            catch
            {
                return View("CreateAssignment");
            }
        }
    }
}
