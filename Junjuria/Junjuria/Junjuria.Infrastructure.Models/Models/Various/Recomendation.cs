﻿namespace Junjuria.Infrastructure.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Recomendation : BaseEntity<int>
    {
        [Required, MaxLength(16)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string Description { get; set; }
        [Required,MinLength(2)]
        public string Author { get; set; }
        public bool IsVerified { get; set; }
    }
}