namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.Common;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using X.PagedList;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManufacturersController : Controller
    {
        private readonly IManufacturersService manufacturersService;

        public ManufacturersController(IManufacturersService manufacturersService)
        {
            this.manufacturersService = manufacturersService;
        }
        public IActionResult Manage(int? pageNum)
        {
            var manufacturers = manufacturersService.GetAllForManaging();
            ViewBag.PageNavigation = manufacturers.Count() > GlobalConstants.MaximumCountOfAllEntitiesOnSinglePageForManaging ? "All" : null;
            var pagedAmmount = manufacturers.ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfAllEntitiesOnSinglePageForManaging);
            return View(pagedAmmount);
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}