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
    }
}