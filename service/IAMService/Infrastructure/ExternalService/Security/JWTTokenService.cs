using Application.Interface.IService;
using Domain.Aggregate;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ExternalService.Security
{
    public class JWTTokenService : ITokenService
    {
        #region Attributes
        private readonly string secretKey;
        private readonly string issuer;
        private readonly string audience;
        private readonly int expiryMinutes;
        private readonly byte[] secretKeyBytes;
        #endregion

        #region Properties
        #endregion

        public JWTTokenService(
            string secretKey,
            string issuer,
            string audience,
            int expiryMinutes)
        {
            this.secretKey = secretKey;
            this.issuer = issuer;
            this.audience = audience;
            this.expiryMinutes = expiryMinutes;
            secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
        }

        #region Methods
        public string GenerateToken(
            List<Privilege> privileges,
            string identityNumber,
            string email,
            string fullName,
            string roleCode)
        {
            // Extract only the names
            var privilegeNames = string.Join(",", privileges.Select(p => p.Name));

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, identityNumber),
                    new Claim(ClaimTypes.Email, email),
                    new Claim("FullName", fullName),
                    new Claim(ClaimTypes.Role, roleCode),
                    new Claim("Privileges", privilegeNames)
                };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken(User user, int expiryMinutes)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Purpose", "PasswordReset"),
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParams, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
