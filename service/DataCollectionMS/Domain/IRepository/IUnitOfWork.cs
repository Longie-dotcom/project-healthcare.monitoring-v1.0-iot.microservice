namespace Domain.IRepository
{
    public interface IUnitOfWork
    {
        T GetRepository<T>() where T : IRepositoryBase;
    }

    public interface IRepositoryBase
    {

    }
}
