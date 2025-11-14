using Domain.Aggregate;

namespace Domain.Entity
{
    public class UserRole
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid UserRoleID { get; private set; }
        public Guid RoleID { get; private set; }
        public Guid UserID { get; private set; }
        public bool IsActive { get; private set; }

        public Role Role { get; private set; }
        public User User { get; private set; }
        #endregion

        protected UserRole() { }

        public UserRole(Guid userRoleId, Guid roleId, Guid userId, bool isActive)
        {
            UserRoleID = userRoleId;
            RoleID = roleId;
            UserID = userId;
            IsActive = isActive;
        }

        #region Methods
        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
        #endregion
    }
}
