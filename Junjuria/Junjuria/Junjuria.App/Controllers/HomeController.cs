namespace Junjuria.App.Controllers
{
    using Junjuria.App.ViewModels;
    using Junjuria.Services.InitialSeed;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : BaseController
    {
        private readonly DataBaseSeeder seeder;

        public HomeController(DataBaseSeeder seeder)
        {
            this.seeder = seeder;
            seeder.SeedData().GetAwaiter().GetResult();
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
    }
}