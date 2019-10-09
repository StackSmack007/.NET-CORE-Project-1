namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.App.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Service")]
    [Authorize(Roles = "Admin,Assistance")]
    public class HomeController : BaseController
    {
      
        public IActionResult Index()
        {
            return View();
        }
    }
}