namespace Junjuria.App.ViewModels.Products
{
using Junjuria.Infrastructure.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
    public class ProductMinorOutDto
    {
        public Grade Grade { get; set; }

        public int Id { get; set; }
        
        [Required, StringLength(maximumLength: 128, MinimumLength = 16)]
        public string Name { get; set; }

        [Required, StringLength(maximumLength: 10240, MinimumLength = 16)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Range(0, 1)]
        public double? Discount { get; set; }

        public bool IsAvailable { get; set; }

        [Url]
        public string MainPicURL { get; set; }

        public int ManufacturerId { get; set; }

        public string ManufacturerName { get; set; }

        public string CategoryTitle { get; set; }
    }
}