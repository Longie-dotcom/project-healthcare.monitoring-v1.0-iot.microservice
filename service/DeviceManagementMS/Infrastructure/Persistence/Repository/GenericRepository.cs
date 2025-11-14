using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Attributes
        protected readonly DeviceManagementDBContext context;
        protected readonly DbSet<T> dbSet;
        #endregion

        #region Properties
        #endregion
        public GenericRepository(DeviceManagementDBContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        #region Methods
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual T Update(Guid id, T entity)
        {
            var existing = dbSet.Find(id);
            if (existing == null)
                throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found");

            context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public virtual void Remove(Guid id)
        {
            var existing = dbSet.Find(id);
            if (existing != null)
                dbSet.Remove(existing);
        }
        #endregion
    }
}
