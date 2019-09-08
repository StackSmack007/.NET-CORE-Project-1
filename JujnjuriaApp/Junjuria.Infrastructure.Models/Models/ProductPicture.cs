namespace Junjuria.Infrastructure.Models.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class ProductPicture
    {
        //TODO Composit Key
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public string PictureURL { get; set; }
    }
}