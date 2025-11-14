namespace Domain.IRepository
{
    public interface IRefreshTokenRepository : IRepositoryBase
    {
        Task<string> AddTokenAsync(Guid id);
        Task DeleteTokenAsync(Guid id);
        Task<string> GetByTokenAsync(string token);
    }
}
