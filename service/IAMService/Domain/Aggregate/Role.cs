using Domain.DomainException;
using Domain.Entity;

namespace Domain.Aggregate
{
    public class Role
    {
        #region Attributes
        private readonly List<RolePrivilege> rolePrivileges = new List<RolePrivilege>();
        #endregion

        #region Properties
        public Guid RoleID { get; private set; }
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }

        public IReadOnlyCollection<RolePrivilege> RolePrivileges
        {
            get { return rolePrivileges.AsReadOnly(); }
        }
        #endregion

        protected Role() { }

        public Role(
            Guid roleId, 
            string name, 
            string code, 
            string description)
        {
            ValidateRoleID(roleId);
            ValidateName(name);
            ValidateCode(code);
            ValidateDescription(description);

            RoleID = roleId;
            Name = name;
            Code = code;
            Description = description;
        }

        #region Methods
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }

        public void AddPrivilege(Guid privilegeId)
        {
            var existingPrivilege = rolePrivileges.FirstOrDefault(r => r.PrivilegeID == privilegeId);
            if (existingPrivilege != null)
            {
                if (!existingPrivilege.IsActive)
                    existingPrivilege.Activate();
            }
            else
            {
                rolePrivileges.Add(new RolePrivilege(
                    Guid.NewGuid(), RoleID, privilegeId, true));
            }
        }

        public void RemovePrivilege(Guid privilegeId)
        {
            var privilege = rolePrivileges.FirstOrDefault(r => r.PrivilegeID == privilegeId);
            if (privilege == null || !privilege.IsActive)
                throw new InvalidRoleAggregateException("Role does not have this privilege.");

            privilege.Deactivate();
        }
        #endregion

        #region Private Validators
        private static void ValidateRoleID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidRoleAggregateException(
                    "RoleID cannot be empty.");
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidRoleAggregateException(
                    "Role name cannot be empty.");
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidRoleAggregateException(
                    "Role code cannot be empty.");
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidRoleAggregateException(
                    "Role description cannot be empty.");
        }
        #endregion
    }
}
