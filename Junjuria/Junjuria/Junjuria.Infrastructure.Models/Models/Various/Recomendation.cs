namespace Junjuria.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Recomendation : BaseEntity<int>
    {
        [Required, MaxLength(16)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string Description { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser Author { get; set; }
    }
}