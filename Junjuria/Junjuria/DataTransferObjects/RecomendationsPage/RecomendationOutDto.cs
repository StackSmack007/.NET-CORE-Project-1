namespace Junjuria.DataTransferObjects.RecomendationsPage
{
    using Junjuria.Common.Interfaces.AutoMapper;
    using Junjuria.Infrastructure.Models;
    using System;
    public class RecomendationOutDto : IMapFrom<Recomendation>
    {

        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateOfCreation { get; set; }

        public string Title { get; set; }

         public string Description { get; set; }

        public string Author { get; set; }
    }
}