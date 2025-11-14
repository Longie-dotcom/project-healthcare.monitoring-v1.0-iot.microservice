using Domain.DomainException;
using Domain.Entity;
using Domain.Enum;
using Domain.ValueObject;
using System.Text.RegularExpressions;

namespace Domain.Aggregate
{
    public class User
    {
        #region Attributes
        private readonly List<UserRole> userRoles = new List<UserRole>();
        private readonly List<UserPrivilege> userPrivileges = new List<UserPrivilege>();
        #endregion

        #region Properties
        public Guid UserID { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public DateTime Dob { get; private set; }
        public string Address { get; private set; }
        public string Gender { get; private set; }
        public string Phone { get; private set; }
        public Password Password { get; private set; }
        public string IdentityNumber { get; private set; }
        public bool IsActive { get; private set; }

        public RefreshToken? RefreshToken { get; private set; }
        public int Age
        {
            get { return CalculateAge(Dob); }
        }
        public IReadOnlyCollection<UserRole> UserRoles 
        { 
            get { return userRoles.AsReadOnly(); } 
        }
        public IReadOnlyCollection<UserPrivilege> UserPrivileges
        {
            get { return userPrivileges.AsReadOnly(); }
        }
        #endregion

        protected User() { }

        public User(
            Guid userID, 
            string email,
            string fullName,
            DateTime dob,
            string address,
            string gender,
            string phone,
            string password, 
            string identityNumber, 
            bool isActive = true)
        {
            ValidateUserID(userID);
            ValidateEmail(email);
            ValidateFullName(fullName);
            ValidateDob(dob);
            ValidateAddress(address);
            ValidateGender(gender);
            ValidatePhone(phone);
            Password = Password.FromPlain(password);
            ValidateIdentityNumber(identityNumber);

            UserID = userID;
            Email = email;
            FullName = fullName;
            Dob = dob;
            Address = address;
            Gender = gender;
            Phone = phone;
            IdentityNumber = identityNumber;
            IsActive = isActive;
        }

        #region Methods
        public void UpdateEmail(string email)
        {
            ValidateEmail(email);
            Email = email;
        }

        public void UpdateFullName(string fullName)
        {
            ValidateFullName(fullName);
            FullName = fullName;
        }

        public void UpdateDob(DateTime dob)
        {
            ValidateDob(dob);
            Dob = dob;
        }

        public void UpdateAddress(string address)
        {
            ValidateAddress(address);
            Address = address;
        }

        public void UpdateGender(string gender)
        {
            ValidateGender(gender);
            Gender = gender;
        }

        public void UpdatePhone(string phone)
        {
            ValidatePhone(phone);
            Phone = phone;
        }

        public void ChangePassword(string newPassword)
        {
            Password = Password.FromPlain(newPassword);
        }

        public bool CheckPassword(string plain)
        {
            return Password.Verify(plain);
        }

        public void UpdateActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void AddGrantedPrivilege(Guid privilegeId, bool isGranted)
        {
            var existingGrantedPrivilege 
                = userPrivileges.FirstOrDefault(r => r.PrivilegeID == privilegeId);
            if (existingGrantedPrivilege == null)
            {
                userPrivileges.Add(new UserPrivilege(
                    Guid.NewGuid(), privilegeId, UserID, true));
            }
        }

        public void RemoveGrantedPrivilege(Guid privilegeId)
        {
            var existingGrantedPrivilege
                = userPrivileges.FirstOrDefault(r => r.PrivilegeID == privilegeId);
            if (existingGrantedPrivilege == null)
                throw new InvalidUserAggregateException(
                    "User does not have this granted privilege.");
            userPrivileges.Remove(existingGrantedPrivilege);
        }

        public void AddRole(Guid roleId)
        {
            var existingRole = userRoles.FirstOrDefault(r => r.RoleID == roleId);
            if (existingRole != null)
            {
                if (!existingRole.IsActive)
                    existingRole.Activate();
            }
            else
            {
                userRoles.Add(new UserRole(
                    Guid.NewGuid(), roleId, UserID, true));
            }
        }

        public void RemoveRole(Guid roleId)
        {
            var role = userRoles.FirstOrDefault(r => r.RoleID == roleId);
            if (role == null || !role.IsActive)
                throw new InvalidUserAggregateException("User does not have this role.");

            role.Deactivate();
        }
        #endregion

        #region Private Validators
        private static void ValidateUserID(Guid id)
        {
            if (id == Guid.Empty) 
                throw new InvalidUserAggregateException(
                    "UserID cannot be empty.");
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidUserAggregateException(
                    "Email cannot be empty.");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new InvalidUserAggregateException(
                    "Email has invalid format.");
        }

        private static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidUserAggregateException(
                    "Full name cannot be empty.");
        }

        private static void ValidateDob(DateTime dob)
        {
            if (dob > DateTime.Today)
                throw new InvalidUserAggregateException(
                    "Date of birth cannot be in the future.");
        }

        private static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidUserAggregateException(
                    "Address cannot be empty.");
        }

        private static void ValidateGender(string gender)
        {
            if (gender != GenderEnum.MALE && gender != GenderEnum.FEMALE)
                throw new InvalidUserAggregateException(
                    "Gender must be male or female.");
        }

        private static void ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new InvalidUserAggregateException(
                    "Phone number cannot be empty.");

            if (!Regex.IsMatch(phone, @"^\+?\d{9,15}$"))
                throw new InvalidUserAggregateException(
                    "Phone number has invalid format.");
        }

        private static void ValidateIdentityNumber(string identityNumber)
        {
            if (string.IsNullOrWhiteSpace(identityNumber))
                throw new InvalidUserAggregateException(
                    "Identity number cannot be empty.");
        }
        #endregion

        #region Private Helpers
        public List<Privilege> GetEffectivePrivileges(Guid roleId)
        {
            // Collect all privileges from active roles
            var privileges = new HashSet<Privilege>(
                userRoles
                    .Where(ur => ur.IsActive && ur.Role != null && ur.RoleID == roleId)
                    .SelectMany(ur => ur.Role.RolePrivileges)
                    .Where(rp => rp.IsActive && rp.Privilege != null)
                    .Select(rp => rp.Privilege)
            );

            // Apply user-specific privilege overrides
            foreach (var up in userPrivileges)
            {
                if (up.Privilege == null)
                    continue;

                if (up.IsGranted)
                {
                    // Add explicitly granted privilege
                    privileges.Add(up.Privilege);
                }
                else
                {
                    // Remove explicitly revoked privilege
                    privileges.RemoveWhere(p => p.PrivilegeID == up.PrivilegeID);
                }
            }

            return privileges.ToList();
        }

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
        #endregion
    }
}
