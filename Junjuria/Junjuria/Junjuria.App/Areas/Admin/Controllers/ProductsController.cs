using Junjuria.App.Controllers;
using Junjuria.Common;
using Junjuria.DataTransferObjects.Admin.Products;
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
        private readonly ICategoryService categoryService;
        private readonly IManufacturerService manufacturerService;

        public ProductsController(IProductService productService, ICategoryService categoryService, IManufacturerService manufacturerService)
        {
            this.productsService = productService;
            this.categoryService = categoryService;
            this.manufacturerService = manufacturerService;
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
            await productsService.MarkProductAsDeleted(productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> UnDelete(int productId)
        {
            await productsService.MarkProductAsNotDeleted(productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStockQuantity(int productId, uint quantity)
        {
            await productsService.SetNewQuantity(productId, quantity);
            return RedirectToAction(nameof(Manage));
        }

        public IActionResult AddProduct()
        {
            ViewData["Categories"] = categoryService.GetAllMinified();
            ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
            return this.View(new NewProductInDto());
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(NewProductInDto dto)
        {
            if (dto.PicturesCount > dto.ProductPictures.Count || dto.CharacteristicsCount > dto.Characteristics.Count)
            {
                if (dto.PicturesCount > dto.ProductPictures.Count) dto.ProductPictures.Add(new NewProductPictureDto());
                if (dto.CharacteristicsCount > dto.Characteristics.Count) dto.Characteristics.Add(new NewProductCharacteristicDto());
                ViewData["Categories"] = categoryService.GetAllMinified();
                ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
                return View(dto);
            }

            if (ModelState.IsValid)
            {
                await productsService.AddNewProduct(dto);
            }
            ViewData["Categories"] = categoryService.GetAllMinified();
            ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
            return this.View(dto);
        }

    }
}