using System.ComponentModel.DataAnnotations.Schema;

namespace Junjuria.Infrastructure.Models
{
    public class UserFavouriteProduct: BaseEntityData
    {
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User{get;set;}

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}