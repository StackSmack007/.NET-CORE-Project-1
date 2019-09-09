namespace Junjuria.Infrastructure.Models
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Order : BaseEntity<string>
    {
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual AppUser Customer { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal DeliveryFee { get; set; }

        public Status Status { get; set; }

        public virtual ICollection<ProductOrder> OrderProducts { get; set;}

        [NotMapped]
        public decimal TotalPrice => OrderProducts.Select(x => (x.Quantity) * (x.Product.Price)).Sum();

        [NotMapped]
        public double TotalWeight => OrderProducts.Select(x => (x.Quantity) * (x.Product.Weight)).Sum();
    }
}