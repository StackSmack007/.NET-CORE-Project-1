namespace Junjuria.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Log : BaseEntity<int>
    {
        [Required, MaxLength(32)]
        public string EventName { get; set; }
        [Required, MaxLength(256)]
        public string EventDescription { get; set; }
    }
}