namespace Junjuria.Services.Services.Contracts
{
    public interface IManufacturerService
    {
        System.Collections.Generic.ICollection<DataTransferObjects.Admin.Manufacturers.ManufacturerMiniOutDto> GetAllMinified();
    }
}