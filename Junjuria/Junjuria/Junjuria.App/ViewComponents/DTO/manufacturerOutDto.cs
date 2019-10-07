namespace Junjuria.App.ViewComponents.DTO
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    public class ManufacturerOutDto : IMapFrom<Manufacturer>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductsCount { get; set; }
    }
}
