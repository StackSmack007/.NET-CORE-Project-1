namespace Junjuria.Infrastructure.Models.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Grades = new HashSet<ProductVote>();
        }

        [Required, MaxLength(256)]
        public string Adress { get; set; }

        [Required, MaxLength(32)]
        public string Town { get; set; }
        
        public virtual ICollection<ProductVote> Grades { get; set; }
    }
}