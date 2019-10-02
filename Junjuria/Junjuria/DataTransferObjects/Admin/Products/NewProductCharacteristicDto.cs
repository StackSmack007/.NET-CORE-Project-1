namespace Junjuria.DataTransferObjects.Admin.Products
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    public class NewProductCharacteristicDto : IComparable<NewProductCharacteristicDto>, IMapTo<ProductCharacteristic>
    {
        [Required]
        public string Title { get; set; }

        [MaxLength(64)]
        public string TextValue { get; set; }

        public double? NumericValue { get; set; }

        public int CompareTo(NewProductCharacteristicDto other)
        {
            if (other.Title == this.Title)
            {
                return 0;
            }
            return 1;
        }
    }
}