using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;

namespace Junjuria.DataTransferObjects.Admin.Manufacturers
{
    public class ManufacturerManageInfoOutData: ManufacturerMiniOutDto,IMapFrom<Manufacturer>
    {
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

         public string WebAddress { get; set; }

        public int ProductsCount { get; set; }
    }
}