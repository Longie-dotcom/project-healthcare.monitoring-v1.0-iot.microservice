using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Attributes
        private readonly DataCollectionDBContext context;
        private readonly IServiceProvider provider;
        private readonly Dictionary<Type, object> repositories = new();
        #endregion

        #region Properties
        #endregion

        public UnitOfWork(
            DataCollectionDBContext context,
            IServiceProvider provider)
        {
            this.context = context;
            this.provider = provider;
        }

        #region Methods
        public T GetRepository<T>() where T : IRepositoryBase
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                // Resolve from DI
                repo = (IRepositoryBase)provider.GetRequiredService(type);

                // Cache it
                repositories[type] = repo;
            }

            return (T)repo;
        }
        #endregion
    }
}
