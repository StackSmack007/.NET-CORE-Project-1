namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.App.Controllers;
    using Junjuria.App.Hubs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Assistance")]
    [Authorize(Roles = "Admin,Assistance")]
    public class HomeController : BaseController
    {
      
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult MakeChatTab(string userName)
        {
            return PartialView("_miniChatWindowPartial", userName);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult MakeChatMessage([FromBody] ResponseMessage response)
        {
            return PartialView("_commentPartial", response);
        }

    }
}