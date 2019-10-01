using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Junjuria.DataTransferObjects.Admin.Products
{
    public class NewProductCharacteristicDto:IMapTo<ProductCharacteristic>
    {
        [Required]
        public string Title { get; set; }

        [MaxLength(64)]
        public string TextValue { get; set; }

        public double? NumericValue { get; set; }
    }
}