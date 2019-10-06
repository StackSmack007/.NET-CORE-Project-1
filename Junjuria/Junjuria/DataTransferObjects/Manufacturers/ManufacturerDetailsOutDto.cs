namespace Junjuria.DataTransferObjects.Manufacturers
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.Collections.Generic;

    public class ManufacturerDetailsOutDto : IMapFrom<Manufacturer>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string WebAddress { get; set; }

        public virtual ICollection<ManufacturerProductMiniOutDto> Products { get; set; }
    }
}