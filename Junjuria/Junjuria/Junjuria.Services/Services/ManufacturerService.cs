namespace Junjuria.Services.Services
{
    using AutoMapper;
    using Junjuria.Common.Extensions;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.Infrastructure.Models;
    using Junjuria.Services.Services.Contracts;
    using System.Collections.Generic;
    using System.Linq;

    public class ManufacturerService : IManufacturerService
    {
        private readonly IRepository<Manufacturer> manufacturerRepository;
        private readonly IMapper mapper;

        public ManufacturerService(IRepository<Manufacturer> manufacturerRepository, IMapper mapper)
        {
            this.manufacturerRepository = manufacturerRepository;
            this.mapper = mapper;
        }

        public ICollection<ManufacturerMiniOutDto> GetAllMinified()
        {
            return manufacturerRepository.All().To<ManufacturerMiniOutDto>().ToArray();
        }
    }
}