namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.DataTransferObjects.Manufacturers;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManufacturersService : IManufacturersService
    {
        private readonly IRepository<Manufacturer> manufacturerRepository;
        private readonly IMapper mapper;

        public ManufacturersService(IRepository<Manufacturer> manufacturerRepository, IMapper mapper)
        {
            this.manufacturerRepository = manufacturerRepository;
            this.mapper = mapper;
        }

        public IQueryable<ManufacturerManageInfoOutData> GetAllForManaging() => manufacturerRepository.All().To<ManufacturerManageInfoOutData>();

        public ICollection<ManufacturerMiniOutDto> GetAllMinified()
        {
            return manufacturerRepository.All().To<ManufacturerMiniOutDto>().ToArray();
        }

        public async Task<ManufacturerDetailsOutDto> GetByIdAsync(int id)
        {
            var manufacturer = await manufacturerRepository.All()
                                                          .Where(x => x.Id == id)
                                                          .To<ManufacturerDetailsOutDto>()
                                                          .FirstOrDefaultAsync();
            return manufacturer;
        }

        public string GetNameById(int id) => manufacturerRepository.All().FirstOrDefault(x => x.Id == id).Name;
    }
}