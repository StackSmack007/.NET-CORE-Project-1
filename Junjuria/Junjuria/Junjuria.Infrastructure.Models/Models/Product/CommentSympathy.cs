namespace Junjuria.Infrastructure.Models
{
    using Junjuria.Infrastructure.Models.Enumerations;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class CommentSympathy
    {
        //TODO Add composite key Sympathiser and Comment
        public string SympathiserId { get; set; }
        [ForeignKey(nameof(SympathiserId))]
        public virtual AppUser Sympathiser { get; set; }

        public int CommentId { get; set; }
        [ForeignKey(nameof(CommentId))]
        public virtual ProductComment Comment { get; set; }

        public Attitude Attitude { get; set; }
    }
}