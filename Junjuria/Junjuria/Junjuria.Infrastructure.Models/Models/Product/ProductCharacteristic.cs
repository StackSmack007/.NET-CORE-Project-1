namespace Junjuria.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductCharacteristic : BaseEntityData
    {
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public string Title { get; set; }

        [MaxLength(64)]
        public string TextValue { get; set; }

        public double? NumericValue { get; set; }
    }
}