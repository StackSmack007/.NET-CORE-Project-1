namespace Junjuria.Services.Services.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Junjuria.DataTransferObjects.Admin.Manufacturers;
    using Junjuria.DataTransferObjects.Manufacturers;

    public interface IManufacturersService
    {
        ICollection<DataTransferObjects.Admin.Manufacturers.ManufacturerMiniOutDto> GetAllMinified();
        Task<ManufacturerDetailsOutDto> GetByIdAsync(int id);
        IQueryable<ManufacturerManageInfoOutData> GetAllForManaging();
        string GetNameById(int id);
        Task<bool> NameTaken(string name, int ownerId = 0);
        void CreateNewManufacturer(ManufacturerInDto dto);
        Task SetManufacturerAsDeletedAsync(int id);
        Task SetManufacturerAsUnDeletedAsync(int id);
        Task<ManufacturerEditDto> GetManufacturerForEditingAsync(int id);
        void EditManufacturer(ManufacturerEditDto dto);
    }
}