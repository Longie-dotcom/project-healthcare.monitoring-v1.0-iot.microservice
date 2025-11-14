using Domain.Aggregate;

namespace Domain.Entity
{
    public class UserPrivilege
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid UserPrivilegeID { get; private set; }
        public Guid PrivilegeID { get; private set; }
        public Guid UserID { get; private set; }
        public bool IsGranted { get; private set; }

        public Privilege Privilege { get; private set; }
        public User User { get; private set; }
        #endregion

        protected UserPrivilege() { }

        public UserPrivilege(
            Guid userPrivilegeId, Guid privilegeId, Guid userId, bool isGranted)
        {
            UserPrivilegeID = userPrivilegeId;
            PrivilegeID = privilegeId;
            UserID = userId;
            IsGranted = isGranted;
        }

        #region Methods
        public void UpdateGranted(bool isGranted)
        {
            IsGranted = isGranted;
        }
        #endregion
    }
}
