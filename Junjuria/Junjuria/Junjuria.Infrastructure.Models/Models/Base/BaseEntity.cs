namespace Junjuria.Infrastructure.Models
{
    public abstract class BaseEntity<T>:BaseEntityData
    {
        public T Id { get; set; }
    }
}