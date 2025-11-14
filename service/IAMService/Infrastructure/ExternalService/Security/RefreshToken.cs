using Domain.Aggregate;

namespace Infrastructure.ExternalService.Security
{
    public class RefreshToken
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public User User { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
    }
}
