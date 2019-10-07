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

        public void CreateNewManufacturer(ManufacturerInDto dto)
        {
            var manufacturer = mapper.Map<Manufacturer>(dto);
            lock (ConcurencyMaster.LockManufacturersObj)
            {
                if (!NameTaken(dto.Name).GetAwaiter().GetResult())
                {
                    manufacturerRepository.AddAssync(manufacturer).GetAwaiter().GetResult();
                    manufacturerRepository.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }
        }
        public void EditManufacturer(ManufacturerEditDto dto)
        {
            var manufacturer = manufacturerRepository.All().FirstOrDefault(x => x.Id == dto.Id);
            if (manufacturer is null) return;
            lock (ConcurencyMaster.LockManufacturersObj)
            {
                if (NameTaken(dto.Name,dto.Id).GetAwaiter().GetResult()) return;
                manufacturer.Name = dto.Name;
                manufacturer.Email = dto.Email;
                manufacturer.PhoneNumber = dto.PhoneNumber;
                manufacturer.WebAddress = dto.WebAddress;
                manufacturerRepository.SaveChangesAsync().GetAwaiter().GetResult();
            }
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

        public async Task<ManufacturerEditDto> GetManufacturerForEditingAsync(int id)
        {
            var manufacturer = await manufacturerRepository.All()
                                                          .Where(x => x.Id == id)
                                                          .To<ManufacturerEditDto>()
                                                          .FirstOrDefaultAsync();
            return manufacturer;
        }

        public string GetNameById(int id) => manufacturerRepository.All().FirstOrDefault(x => x.Id == id).Name;

        public async Task<bool> NameTaken(string name, int ownerId = 0) =>
               await manufacturerRepository.All().AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != ownerId);


        public async Task SetManufacturerAsDeletedAsync(int id)
        {
            var manufacturer = await manufacturerRepository.All().FirstOrDefaultAsync(x => x.Id == id);
            if (manufacturer is null || manufacturer.IsDeleted) return;
            manufacturer.IsDeleted = true;
            await manufacturerRepository.SaveChangesAsync();
        }
        public async Task SetManufacturerAsUnDeletedAsync(int id)
        {
            var manufacturer = await manufacturerRepository.All().FirstOrDefaultAsync(x => x.Id == id);
            if (manufacturer is null || !manufacturer.IsDeleted) return;
            manufacturer.IsDeleted = false;
            await manufacturerRepository.SaveChangesAsync();
        }
    }
}