namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.Common;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using System.Linq;
    using System.Threading.Tasks;
    using X.PagedList;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManufacturersController : Controller
    {
        private readonly IManufacturersService manufacturersService;
        private readonly IMemoryCache memoryCache;

        public ManufacturersController(IManufacturersService manufacturersService, IMemoryCache memoryCache)
        {
            this.manufacturersService = manufacturersService;
            this.memoryCache = memoryCache;
        }
        public IActionResult Manage(int? pageNum)
        {
            var manufacturers = manufacturersService.GetAllForManaging();
            ViewBag.PageNavigation = manufacturers.Count() > GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging ? "Manage" : null;
            var pagedAmmount = manufacturers.ToPagedList(pageNum ?? 1, GlobalConstants.MaximumCountOfRowEntitiesOnSinglePageForManaging);
            return View(pagedAmmount);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ManufacturerInDto dto)
        {
            memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
            if (ModelState.IsValid)
            {
                if (await manufacturersService.NameTaken(dto.Name))
                {
                    ModelState.AddModelError("Name", $"Name {dto.Name} is already used!");
                    return View(dto);
                }
                    manufacturersService.CreateNewManufacturer(dto);
                    return RedirectToAction(nameof(Manage));
            }
            return View(dto);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            ManufacturerEditDto dto = await manufacturersService.GetManufacturerForEditingAsync(id);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManufacturerEditDto dto)
        {   
            if (ModelState.IsValid)
            {
                memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
                if (await manufacturersService.NameTaken(dto.Name,dto.Id))
                {
                    ModelState.AddModelError("Name", $"Name {dto.Name} is already used!");
                    return View(dto);
                }
                manufacturersService.EditManufacturer(dto);
                return RedirectToAction(nameof(Manage));
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
            await manufacturersService.SetManufacturerAsDeletedAsync(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> UnDelete(int id)
        {
            memoryCache.Remove(GlobalConstants.CasheManufactorersInButtonName);
            await manufacturersService.SetManufacturerAsUnDeletedAsync(id);
            return RedirectToAction(nameof(Manage));
        }

    }
}