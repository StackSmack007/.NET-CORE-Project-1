using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;

namespace Junjuria.DataTransferObjects.Admin.Manufacturers
{
    public class ManufacturerMiniOutDto : IMapFrom<Manufacturer>
    {
       public int Id { get; set; }
       public string Name { get; set; }
    }
}