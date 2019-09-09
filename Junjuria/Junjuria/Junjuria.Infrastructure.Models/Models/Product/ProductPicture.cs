namespace Junjuria.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductPicture
    {
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public string PictureURL { get; set; }
    }
}