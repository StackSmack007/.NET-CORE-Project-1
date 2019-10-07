namespace Junjuria.DataTransferObjects.Admin.Manufacturers
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;
    public class ManufacturerInDto : IMapTo<Manufacturer>
    {

        [Required, MaxLength(128),MinLength(2)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Url]
        public string WebAddress { get; set; }

    }
}