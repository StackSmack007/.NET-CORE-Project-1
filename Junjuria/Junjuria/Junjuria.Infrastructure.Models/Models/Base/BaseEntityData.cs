namespace Junjuria.Infrastructure.Models
{
    using System;
    public abstract class BaseEntityData
    {
        public DateTime DateOfCreation { get; set; }
        public bool IsDeleted { get; set; }
        protected BaseEntityData()
        {
            DateOfCreation = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}