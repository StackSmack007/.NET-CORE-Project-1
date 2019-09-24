namespace Junjuria.App.Controllers
{
    using DataTransferObjects;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;


    public class HomeController : BaseController
    {
         //public HomeController(DataBaseSeeder seeder)
        //{
        //     seeder.SeedData().GetAwaiter().GetResult();
        //}

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
    }
}