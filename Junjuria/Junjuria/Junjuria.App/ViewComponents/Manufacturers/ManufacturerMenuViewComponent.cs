namespace Junjuria.App.ViewComponents.Categories
{
    using Junjuria.App.ViewComponents.DTO;
    using Junjuria.Common.Extensions;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManufacturerMenuViewComponent : ViewComponent
    {
        private readonly IRepository<Manufacturer> manufacturerRepository;

        public ManufacturerMenuViewComponent(IRepository<Manufacturer> manufacturerRepository)
        {
            this.manufacturerRepository = manufacturerRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var manufacturersDtos = manufacturerRepository.All().Where(x=>!x.IsDeleted&&x.Products.Any()).To<ManufacturerOutDto>().OrderBy(x=>x.Name).ToArray();
            return await Task.Run(() => this.View(manufacturersDtos));
        }
    }
}