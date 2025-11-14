using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.InfrastructureException;
using Infrastructure.Persistence.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repository
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : AggregateBase
    {
        #region Attributes
        protected readonly IMongoCollection<T> collection;
        #endregion

        #region Properties
        #endregion

        public GenericRepository(DataCollectionDBContext context, string collectionName)
        {
            collection = context.GetCollection<T>(collectionName);
        }

        #region Methods
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await collection.Find(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            var all = await GetAllAsync();
            return all.Where(predicate);
        }

        public void Add(T entity)
        {
            collection.InsertOne(entity);
        }

        public T? Update(Guid id, T entity)
        {
            var result = collection.ReplaceOne(x => x.ID == id, entity);
            if (result.MatchedCount == 0)
                throw new RepositoryException($"Entity with ID:{id} not found");
            return entity;
        }

        public bool Remove(Guid id)
        {
            var result = collection.DeleteOne(x => x.ID == id);
            return result.DeletedCount > 0;
        }
        #endregion
    }
}
