namespace Junjuria.Infrastructure.Models
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Product : BaseEntity<int>
    {
        public Product()
        {
            Votes = new HashSet<ProductVote>();
            ProductPictures = new HashSet<ProductPicture>();
            ProductComments = new HashSet<ProductComment>();
            Characteristics = new HashSet<ProductCharacteristic>();
            ProductOrders = new HashSet<ProductOrder>();
        }

        public Grade Grade => Votes.Any()?(Grade)((int)Math.Round((double)Votes.Sum(x => (int)x.Grade) / Votes.Count())):Grade.NotRated;

        [Required, StringLength(maximumLength: 128, MinimumLength = 16)]
        public string Name { get; set; }

        [Required, StringLength(maximumLength: 10240, MinimumLength = 16)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Range(0, 1)]
        public double Discount { get; set; }

        public uint Quantity { get; set; }

        [Url]
        public string ReviewURL { get; set; }

        [Url]
        public string MainPicURL { get; set; }

        public uint? MonthsWarranty { get; set; }

        public double Weight { get; set; }

        public int ManufacturerId { get; set; }
        [ForeignKey(nameof(ManufacturerId))]
        public virtual Manufacturer Manufacturer { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        public virtual ICollection<ProductVote> Votes { get; set; }

        public virtual ICollection<ProductPicture> ProductPictures { get; set; }

        public virtual ICollection<ProductComment> ProductComments { get; set; }

        public virtual ICollection<ProductCharacteristic> Characteristics { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}