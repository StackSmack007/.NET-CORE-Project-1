namespace Junjuria.Infrastructure.Models.Models
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ProductVote
    {
        //TODO COMPOSIT KEY!
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser Voter { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public Grade Grade { get; set; }
    }
}