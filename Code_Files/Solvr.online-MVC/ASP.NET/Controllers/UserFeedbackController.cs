using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers
{
    public class EUserFeedbackController : Controller
    {
        [Route("success")]
        public ActionResult Success()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.ResponseStyleClass = TempData["ResponseStyleClass"];
            ViewBag.ButtonText = TempData["ButtonText"];
            ViewBag.ButtonLink = TempData["ButtonLink"];
            ViewBag.PageTitle = TempData["PageTitle"];
            ViewBag.SubMessage = TempData["SubMessage"];
            ViewBag.Image = TempData["Image"];
            return View("UserFeedback");
        }

        [Route("nothing-found")]
        public ActionResult NothingFound()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.ResponseStyleClass = TempData["ResponseStyleClass"];
            ViewBag.ButtonText = TempData["ButtonText"];
            ViewBag.ButtonLink = TempData["ButtonLink"];
            ViewBag.PageTitle = TempData["PageTitle"];
            ViewBag.SubMessage = TempData["SubMessage"];
            ViewBag.Image = TempData["Image"];
            return View("UserFeedback");
        }
    }
}
