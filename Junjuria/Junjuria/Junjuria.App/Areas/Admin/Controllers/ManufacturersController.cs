namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.Common;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
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
            await manufacturersService.SetManufacturerAsDeletedAsync(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        public async Task<IActionResult> UnDelete(int id)
        {
            await manufacturersService.SetManufacturerAsUnDeletedAsync(id);
            return RedirectToAction(nameof(Manage));
        }

    }
}