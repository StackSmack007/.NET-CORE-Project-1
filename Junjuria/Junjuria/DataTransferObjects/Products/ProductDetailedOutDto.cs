namespace Junjuria.DataTransferObjects.Products
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.Collections.Generic;
    public class ProductDetailedOutDto
    {
        public int Id { get; set; }
        public Grade Grade { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public double Discount { get; set; }

        public uint Quantity { get; set; }

        public string ReviewURL { get; set; }

        public string MainPicURL { get; set; }

        public uint? MonthsWarranty { get; set; }

        public double Weight { get; set; }

        public int ManufacturerId { get; set; }

        public virtual string ManufacturerName { get; set; }

        public virtual string CategoryTitle { get; set; }
        public virtual int CategoryId { get; set; }

        public virtual ICollection<string> VotesrsNames { get; set; }

        public virtual ICollection<string> ProductPictures { get; set; }

        public virtual ICollection<ProductCommentDtoOut> ProductComments { get; set; }

        public virtual ICollection<ProductCharacteristicOutDto> Characteristics { get; set; }
    }
}