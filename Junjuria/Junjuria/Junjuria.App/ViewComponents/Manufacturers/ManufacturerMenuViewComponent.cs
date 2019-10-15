namespace Junjuria.App.ViewComponents.Categories
{
    using Junjuria.App.ViewComponents.DTO;
    using Junjuria.Common;
    using Junjuria.Common.Extensions;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    public class ManufacturerMenuViewComponent : ViewComponent
    {
        private readonly IRepository<Manufacturer> manufacturerRepository;
        private MemoryCacheEntryOptions cacheOptions;
        private readonly IMemoryCache memoryCache;
       

        public ManufacturerMenuViewComponent(IRepository<Manufacturer> manufacturerRepository, IMemoryCache memoryCache)
        {
            this.manufacturerRepository = manufacturerRepository;
            this.memoryCache = memoryCache;
            this.cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromHours(2))
                 .SetPriority(CacheItemPriority.Normal);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ManufacturerOutDto[] manufacturersDtos;
            if (!memoryCache.TryGetValue(GlobalConstants.CasheManufactorersInButtonName, out manufacturersDtos))
            {
                manufacturersDtos = manufacturerRepository.All()
                                                          .Where(x => !x.IsDeleted && x.Products.Any())
                                                          .To<ManufacturerOutDto>()
                                                          .OrderBy(x => x.Name)
                                                          .ToArray();
                memoryCache.Set(GlobalConstants.CasheManufactorersInButtonName, manufacturersDtos, cacheOptions);
            }
            return await Task.Run(() => this.View(manufacturersDtos));
        }
    }
}