namespace Domain.IRepository
{
    public interface IGenericRepository<T>
        where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);

        void Add(T entity);
        T? Update(Guid id, T entity);
        bool Remove(Guid id);
    }
}
