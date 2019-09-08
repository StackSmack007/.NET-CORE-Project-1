namespace Junjuria.Infrastructure.Models.Models.Base
{
    public abstract class BaseEntity<T>:BaseCreateTimeTrack
    {
        public T Id { get; set; }
    }
}