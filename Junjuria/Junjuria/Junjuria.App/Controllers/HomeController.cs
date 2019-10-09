namespace Junjuria.App.Controllers
{
    using DataTransferObjects;
    using Junjuria.DataTransferObjects.Products;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : BaseController
    {
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

        public IActionResult ContactUs()
        {           
            return View();
        }

        [HttpPost]
        public string Test(CommentCreateInDto dto)
        {
         return dto.Comment+5*dto.ProductId+"Result";
        }

    }
}