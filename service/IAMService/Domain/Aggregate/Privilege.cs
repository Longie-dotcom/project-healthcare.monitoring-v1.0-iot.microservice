using Domain.DomainException;

namespace Domain.Aggregate
{
    public class Privilege
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid PrivilegeID { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; }
        #endregion

        protected Privilege() { }

        public Privilege(Guid privilegeID, string name, string description)
        {
            ValidatePrivilegeID(privilegeID);
            ValidateName(name);
            ValidateDescription(description);

            PrivilegeID = privilegeID;
            Name = name;
            Description = description;
        }

        #region Methods
        public void UpdateNameAndDescription(string name, string description)
        {
            ValidateName(name);
            ValidateDescription(description);
            Name = name;
            Description = description;
        }
        #endregion

        #region Private Validators
        private static void ValidatePrivilegeID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidPrivilegeAggregateException(
                    "PrivilegeID cannot be empty.");
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidPrivilegeAggregateException(
                    "Privilege name cannot be empty.");
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidPrivilegeAggregateException(
                    "Privilege description cannot be empty.");
        }
        #endregion
    }
}
