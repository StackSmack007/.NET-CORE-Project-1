namespace Junjuria.DataTransferObjects.RecomendationsPage
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;
    public class RecomendationInDto : IMapTo<Recomendation>
    {
        [Required, MaxLength(16)]
        public string Title { get; set; }

        [Required, MaxLength(512)]
        public string Description { get; set; }

        [Required, MinLength(2)]
        public string Author { get; set; }

        public bool IsVerified { get; set; }
    }
}