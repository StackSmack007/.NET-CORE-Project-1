namespace Junjuria.Services.Services
{
    using Junjuria.Infrastructure.Data;
    using Junjuria.Services.Services.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DbRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, new()
    {

        private ApplicationDbContext context;
        private DbSet<TEntity> dbSet;

        public DbRepository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public Task AddAssync(TEntity entity)
        {
            return dbSet.AddAsync(entity);
        }

        public Task AddRangeAssync(IEnumerable<TEntity> entities)
        {
            return dbSet.AddRangeAsync(entities);
        }

        public IQueryable<TEntity> All()
        {
            return dbSet;
        }

        public void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}