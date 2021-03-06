﻿using Junjuria.App.Controllers;
using Junjuria.Common;
using Junjuria.DataTransferObjects.Admin.Products;
using Junjuria.Services.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IManufacturersService manufacturerService;
        private readonly IMemoryCache memoryCache;

        public ProductsController(IProductService productService,
            ICategoryService categoryService,
            IManufacturersService manufacturerService,
            IMemoryCache memoryCache)
        {
            this.productsService = productService;
            this.categoryService = categoryService;
            this.manufacturerService = manufacturerService;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index() => RedirectToAction(nameof(Manage));
        public IActionResult Manage(int? pageNum)
        {
            ViewBag.PageNavigation = productsService.GetAll().Count() > GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging ? "All" : null;
            var dtos = productsService.GetAllForManaging().ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging);
            return this.View(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            memoryCache.Remove(GlobalConstants.CasheCategoriesInButtonName);
            memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
            await productsService.MarkProductAsDeletedAsync(productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> UnDelete(int productId)
        {
            memoryCache.Remove(GlobalConstants.CasheCategoriesInButtonName);
            memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
            await productsService.MarkProductAsNotDeletedAsync(productId);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStockQuantity(int productId, uint quantity)
        {
            await productsService.SetNewQuantityAsync(productId, quantity);
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
            if (!string.IsNullOrEmpty(dto.ChangeCount))
            {
                if (dto.ChangeCount == "pic++") dto.ProductPictures.Add(new NewProductPictureDto());
                if (dto.ChangeCount == "char++") dto.Characteristics.Add(new NewProductCharacteristicDto());

                if (dto.ChangeCount == "pic--") dto.ProductPictures.RemoveAt(dto.ProductPictures.Count - 1);
                if (dto.ChangeCount == "char--") dto.Characteristics.RemoveAt(dto.Characteristics.Count - 1);
                ViewData["Categories"] = categoryService.GetAllMinified();
                ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
                return View(dto);
            }
            if (ModelState.IsValid)
            {
                memoryCache.Remove(GlobalConstants.CasheCategoriesInButtonName);
                memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
                await productsService.AddNewProductAsync(dto);
                return RedirectToAction(nameof(Manage));
            }
            ViewData["Categories"] = categoryService.GetAllMinified();
            ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
            return this.View(dto);
        }

        private int GetTargetIndex(string input)
        {
            int startIndex = input.IndexOf("#") + 1;
            return int.Parse(input.Substring(startIndex));
        }

        public async Task<IActionResult> Edit(int id)
        {
            EditProductOutDto productDto = await productsService.GetEditableProductAsync(id);
            ViewData["Categories"] = categoryService.GetAllMinified();
            ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductOutDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ChangeCount))
            {
                if (dto.ChangeCount == "pic++") dto.ProductPictures.Add(new NewProductPictureDto());
                if (dto.ChangeCount == "char++") dto.Characteristics.Add(new NewProductCharacteristicDto());
                if (dto.ChangeCount.StartsWith("com++")) dto.ProductComments[GetTargetIndex(dto.ChangeCount)].IsDeleted = false;

                if (dto.ChangeCount.StartsWith("pic--")) dto.ProductPictures.RemoveAt(GetTargetIndex(dto.ChangeCount));
                if (dto.ChangeCount.StartsWith("char--")) dto.Characteristics.RemoveAt(GetTargetIndex(dto.ChangeCount));
                if (dto.ChangeCount.StartsWith("com--")) dto.ProductComments[GetTargetIndex(dto.ChangeCount)].IsDeleted = true;
                bool validState = ModelState.IsValid;
                var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();

                ViewData["Categories"] = categoryService.GetAllMinified();

                ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
                return View(dto);
            }

            if (ModelState.IsValid)
            {
                memoryCache.Remove(GlobalConstants.CasheCategoriesInButtonName);
                memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
                await productsService.ModifyProductAsync(dto);
                return RedirectToAction("Details", "Products", new { area = "", id = dto.Id });
            }
            ViewData["Categories"] = categoryService.GetAllMinified();
            ViewData["Manufacturers"] = manufacturerService.GetAllMinified();
            return this.View(dto);
        }
    }
}