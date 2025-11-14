namespace Domain.Aggregate
{
    public class RefreshToken
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid RefreshTokenID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;

        public User User { get; set; }
        #endregion

        protected RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt)
        {
            UserID = userId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            IsRevoked = false;
        }

        #region Methods
        public void Revoke()
        {
            IsRevoked = true;
        }
        #endregion
    }
}
