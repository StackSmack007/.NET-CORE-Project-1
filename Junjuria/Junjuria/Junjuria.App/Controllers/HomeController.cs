namespace Junjuria.App.Controllers
{
    using Junjuria.App.ViewModels;
    using Junjuria.Infrastructure.Data;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.InitialSeed;
    using Junjuria.Services.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly DataBaseSeeder seeder;

        public HomeController(DataBaseSeeder seeder)
        {
            this.seeder = seeder;
        }

        public IActionResult Index()
        {
            seeder.SeedData().GetAwaiter().GetResult();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
