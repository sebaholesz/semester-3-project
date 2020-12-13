using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
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
                    string urlGetUserCredit = "https://localhost:44316/apiV1/user/get-credit/" + userId;
                    string urlGetUserConcurrencyStamp = "https://localhost:44316/apiV1/user/get-concurrency-stamp/" + userId;
                    HttpResponseMessage urlGetUserUserConcurrencyStampRM = (client.GetAsync(urlGetUserConcurrencyStamp).Result);
                    HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);

                    if (urlGetUserCreditRM.IsSuccessStatusCode && urlGetUserUserConcurrencyStampRM.IsSuccessStatusCode)
                    {
                        ViewBag.Credits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);
                        ViewBag.userId = userId;
                        ViewBag.ConcurrencyStamp = urlGetUserUserConcurrencyStampRM.Content.ReadAsStringAsync().Result;
                        return View("AddCredits");
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
        [HttpPost]
        public ActionResult AddCredits(IFormCollection collection, int credit, string userId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = new User() { Credit = credit, ConcurrencyStamp=collection["ConcurrencyStamp"] };

                    string urlAddCredits = "https://localhost:44316/apiV1/user/add-credit/" + userId;
                    HttpResponseMessage urlAddCreditsRM = client.PutAsync(urlAddCredits, new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;
                    if (urlAddCreditsRM.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Credit added successfully";
                        ViewBag.ResponseStyleClass = "text-success";
                        ViewBag.ButtonText = "Post assignment";
                        ViewBag.ButtonLink = "/assignment/create-assignment";
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

                throw e;
            }
            
        }

    }
}
