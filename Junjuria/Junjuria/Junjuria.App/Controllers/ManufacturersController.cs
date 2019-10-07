namespace Junjuria.App.Controllers
{
    using Junjuria.DataTransferObjects.Manufacturers;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class ManufacturersController : Controller
    {

        private readonly IManufacturersService manufacturersService;

        public ManufacturersController(IManufacturersService manufacturersService)
        {
            this.manufacturersService = manufacturersService;
        }


        public async Task<IActionResult> Details(int id)
        {
            ManufacturerDetailsOutDto info = await manufacturersService.GetByIdAsync(id);

            return View(info);
        }

    }
}