using Domain.Aggregate;
using System.Security.Claims;

namespace Application.Interface.IService
{
    public interface ITokenService
    {
        string GenerateToken(
            List<Privilege> privileges,
            string identityNumber, 
            string email, 
            string fullName, 
            string roleCode);

        public string GeneratePasswordResetToken(User user, int expiryMinutes);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
        bool ValidateToken(string token);
    }
}
