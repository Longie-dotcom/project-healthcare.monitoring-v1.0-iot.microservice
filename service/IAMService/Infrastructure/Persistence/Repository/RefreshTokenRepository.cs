using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.InfrastructureException;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Infrastructure.Persistence.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        #region Attributes
        private readonly IAMDBContext context;
        #endregion

        #region Properties
        #endregion

        public RefreshTokenRepository(IAMDBContext context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<string> AddTokenAsync(Guid userId)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
                throw new RepositoryException("User not found when adding refresh token.");

            // Generate new token
            string newToken = GenerateRefreshToken();

            // Check if a refresh token already exists
            var existingToken = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserID == userId);

            if (existingToken != null)
            {
                existingToken.Token = newToken;
                existingToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                existingToken.CreatedAt = DateTime.UtcNow;
                existingToken.IsRevoked = false;
            }
            else
            {
                var refreshToken = new RefreshToken(userId, newToken, DateTime.UtcNow.AddDays(7));
                await context.RefreshTokens.AddAsync(refreshToken);
            }

            await context.SaveChangesAsync();
            return newToken;
        }

        public async Task DeleteTokenAsync(Guid userId)
        {
            var existingToken = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserID == userId);

            if (existingToken != null)
            {
                context.RefreshTokens.Remove(existingToken);
                await context.SaveChangesAsync();
            }
        }

        public async Task RevokeTokenAsync(Guid userId)
        {
            var existingToken = await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserID == userId);

            if (existingToken != null)
            {
                existingToken.Revoke();
                await context.SaveChangesAsync();
            }
        }

        public async Task<string?> GetByTokenAsync(string token)
        {
            var refreshToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);

            if (refreshToken == null)
                throw new RepositoryException("Refresh token is not found");

            return refreshToken.Token;
        }

        private string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
        #endregion
    }
}
