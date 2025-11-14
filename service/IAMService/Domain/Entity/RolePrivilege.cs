using Domain.Aggregate;

namespace Domain.Entity
{
    public class RolePrivilege
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid RolePrivilegeID { get; private set; }
        public Guid RoleID { get; private set; }
        public Guid PrivilegeID { get; private set; }
        public bool IsActive { get; private set; }

        public Role? Role { get; private set; }
        public Privilege? Privilege { get; private set; }
        #endregion

        protected RolePrivilege() { }

        public RolePrivilege(Guid rolePrivilegeID, Guid roleId, Guid privilegeId, bool isActive)
        {
            RolePrivilegeID = rolePrivilegeID;
            RoleID = roleId;
            PrivilegeID = privilegeId;
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
