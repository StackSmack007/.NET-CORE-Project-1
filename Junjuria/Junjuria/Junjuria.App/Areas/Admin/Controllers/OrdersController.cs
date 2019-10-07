namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.Common;
    using Junjuria.Infrastructure.Models.Enumerations;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using X.PagedList;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private IOrderService ordersService;

        public IActionResult Index() => RedirectToAction(nameof(Manage));

        public OrdersController(IOrderService ordersService)
        {
            this.ordersService = ordersService;
        }

        public IActionResult Manage(int? pageNum)
        {
            ViewBag.PageNavigation = ordersService.GetAllForManaging().Count() > GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging ? "Manage" : null;
            var dtos = ordersService.GetAllForManaging().ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging);
            return this.View(dtos);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string orderId, Status status)
        {
            await ordersService.SetStatus(orderId, status);
            return RedirectToAction(nameof(Manage));
        }
    }
}