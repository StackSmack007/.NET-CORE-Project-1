namespace Junjuria.App.Controllers
{
    using DataTransferObjects;
    using Junjuria.App.Hubs;
    using Junjuria.Infrastructure.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HomeController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        public HomeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> ContactUs()
        {
            var user = await userManager.GetUserAsync(User);
            ViewData["UserId"] = user.Id;
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult MakeChatMessage([FromBody] ResponseMessage response) =>
                             PartialView("_ChatMessageUserPartial", response);




        //[HttpPost]
        //public string Test(CommentCreateInDto dto)
        //{
        // return dto.Comment+5*dto.ProductId+"Result";
        //}

    }
}