using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309/Users");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("/Users").Result;

            IEnumerable<User> users = new List<User>();
            if (response.IsSuccessStatusCode)
            {
                users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
            }

            ViewBag.Users = users;
            return View();
        }

        public ActionResult MyAccount()
        {
            return View();
        }
    }
}