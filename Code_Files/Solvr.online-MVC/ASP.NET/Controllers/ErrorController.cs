using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{statusCode}")]
        public ActionResult ErrorWithStatusCode(int statusCode)
        {
            string errorMessage = "";
            switch (statusCode)
            {
                case 400:
                    errorMessage = "400 - Bad request";
                    break;
                case 404:
                    errorMessage = "404 - Not found";
                    break;
                case 405:
                    errorMessage = "405 - Not allowed";
                    break;
                default:
                    break;
            }

            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

        [Route("error")]
        public ActionResult Error()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View("Error");
        }
    }
}
