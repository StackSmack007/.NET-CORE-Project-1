using Junjuria.App.Controllers;
using Junjuria.Common;
using Junjuria.Services.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Junjuria.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : BaseController
    {
        private readonly IProductService productsService;

        public ProductsController(IProductService productService)
        {
            this.productsService = productService;
        }

        public IActionResult Manage(int? pageNum)
        {
            ViewBag.PageNavigation = productsService.GetAll().Count() > GlobalConstants.MaximumCountOfAllProductsOnSinglePageForManaging ? "All" : null;
            var dtos = productsService.GetAllForManaging().ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfAllProductsOnSinglePageForManaging);
            return this.View(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            await productsService.MarkProductAsDeleted( productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> UnDelete(int productId)
        {
            await productsService.MarkProductAsNotDeleted(productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStockQuantity(int productId,uint quantity)
        {
            await productsService.SetNewQuantity(productId,quantity);
            return RedirectToAction(nameof(Manage));
        }

        public IActionResult AddProduct(int? picsCount,int?specsCount)
        {
            ViewData["PicsCount"] = picsCount ?? 1;
            ViewData["SpecsCount"] = specsCount ?? 1;
            return this.View();
        }

        [HttpPost]
        public IActionResult AddProduct(NewProductInDto dto)
        {
            return this.View();
        }

    }
}