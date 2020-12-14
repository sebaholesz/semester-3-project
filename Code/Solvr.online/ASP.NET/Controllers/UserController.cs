 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace ASP.NET.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("user/add-credits")]
        [HttpGet]
        public ActionResult AddCredits()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlGetUserCredit = "https://localhost:44316/apiV1/user/get-credit";
                    HttpResponseMessage urlGetUserCreditRM = (client.GetAsync(urlGetUserCredit).Result);

                    if (urlGetUserCreditRM.IsSuccessStatusCode)
                    {
                        ViewBag.Credits = JsonConvert.DeserializeObject<int>(urlGetUserCreditRM.Content.ReadAsStringAsync().Result);
                        ViewBag.userId = user.Id;
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
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

        //TODO refactor
        [Route("user/add-credits")]
        [HttpPost]
        public ActionResult AddCredits(int credit)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    User user = _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                    client.DefaultRequestHeaders.Authorization = AuthenticationController.GetAuthorizationHeaderAsync(_userManager, _signInManager, user).Result;

                    string urlAddCredits = "https://localhost:44316/apiV1/user/add-credit";
                    HttpResponseMessage urlAddCreditsRM = client.PutAsync(urlAddCredits, new StringContent(JsonConvert.SerializeObject(credit), Encoding.UTF8, "application/json")).Result;
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
                    return View("UserFeedback");
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                TempData["ErrorMessage"] = e.Message;
                return Redirect("/error");
            }
        }

    }
}
