namespace Junjuria.App.Areas.Admin.Controllers
{
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
  
    public class ManufacturersController:Controller
    {
        private readonly IManufacturersService manufacturersService;

        public ManufacturersController(IManufacturersService manufacturersService)
        {
            this.manufacturersService = manufacturersService;
        }




    }
}