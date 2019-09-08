namespace Junjuria.Infrastructure.Models.Models.Base
{
    using System;
    public abstract class BaseCreateTimeTrack
    {
        public DateTime DateOfCreation { get; set; }
        protected BaseCreateTimeTrack()
        {
            DateOfCreation = DateTime.UtcNow;
        }
    }
}