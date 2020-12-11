using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASP.NET.Controllers
{
    public class UserController : Controller
    {
        [Route("user/add-credits/{userId}")]
        [HttpGet]
        public ActionResult AddCredits(string userId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.FindFirstValue("JWT"));
                    string urlGetUserCredit = "https://localhost:44316/apiV1/user/get-credit/" + userId;
                    HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);
                    if (urlGetUserCreditRM.IsSuccessStatusCode)
                    {
                        ViewBag.Credits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);
                        return View("AddCredit");
                    }
                    else
                    {
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

        [Route("user/add-credits/{userId}")]
        [HttpPut]
        public ActionResult AddCredits(IFormCollection collection, string userId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.FindFirstValue("JWT"));
                    object credits = collection["credits"];
                    string urlAddCredits = "https://localhost:44316/apiV1/user/add-credit/" + userId;
                    HttpResponseMessage urlAddCreditsRM = client.PutAsync(urlAddCredits, new StringContent(JsonConvert.SerializeObject(credits), Encoding.UTF8, "application/json")).Result;
                    if (urlAddCreditsRM.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Credit added successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                        ViewBag.ButtonText = "Post assignment";
                        ViewBag.ButtonLink = "assignment/create-assignment";
                        ViewBag.PageTitle = "Credit added!";
                        ViewBag.SubMessage = "You can now post assignments";
                        ViewBag.Image = "/assets/icons/success.svg";
                    }
                    else
                    {
                    }
                    return View("UserFeedback");

                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

    }
}
