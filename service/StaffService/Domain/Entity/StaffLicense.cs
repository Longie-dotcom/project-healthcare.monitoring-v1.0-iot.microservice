using Domain.Aggregate;
using Domain.DomainException;

namespace Domain.Entity
{
    public class StaffLicense
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid StaffLicenseID { get; private set; }
        public Guid StaffID { get; private set; }
        public string LicenseNumber { get; private set; }
        public string LicenseType { get; private set; }
        public string IssuedBy { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsValid { get; private set; }

        public Staff Staff { get; protected set; }
        #endregion

        protected StaffLicense() { }

        public StaffLicense(
            Guid staffLicenseID,
            Guid staffID,
            string licenseNumber,
            string licenseType,
            string issuedBy,
            DateTime issueDate,
            DateTime expiryDate)
        {
            ValidateStaffLicenseID(staffLicenseID);
            ValidateStaffID(staffID);
            ValidateLicenseNumber(licenseNumber);
            ValidateLicenseType(licenseType);
            ValidateIssuedBy(issuedBy);
            ValidateExpiryDate(issueDate, expiryDate);

            StaffLicenseID = staffLicenseID;
            StaffID = staffID;
            LicenseNumber = licenseNumber;
            LicenseType = licenseType;
            IssuedBy = issuedBy;
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            IsValid = expiryDate > DateTime.UtcNow;
        }

        #region Methods
        public void UpdateLicenseNumber(string licenseNumber)
        {
            ValidateLicenseNumber(licenseNumber);
            LicenseNumber = licenseNumber;
        }

        public void UpdateLicenseType(string licenseType)
        {
            ValidateLicenseType(licenseType);
            LicenseType = licenseType;
        }

        public void UpdateIssuedBy(string issuedBy)
        {
            ValidateIssuedBy(issuedBy);
            IssuedBy = IssuedBy;
        }

        public void UpdateExpiryDate(DateTime issueDate, DateTime expiryDate)
        {
            ValidateExpiryDate(issueDate, expiryDate);
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
        }

        public void UpdateValidity()
        {
            IsValid = ExpiryDate > DateTime.UtcNow;
        }

        public void Renew(DateTime newExpiryDate)
        {
            if (newExpiryDate <= ExpiryDate)
                throw new InvalidStaffAggregateException("New expiry date must be later than the current one.");

            ExpiryDate = newExpiryDate;
            IsValid = true;
        }
        #endregion

        #region Validator
        private void ValidateStaffLicenseID(Guid staffLicenceId)
        {
            if (staffLicenceId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffLicenseID cannot be empty.");
        }

        private void ValidateStaffID(Guid staffId)
        {
            if (staffId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffID cannot be empty.");
        }

        private void ValidateLicenseNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new InvalidStaffAggregateException("LicenseNumber cannot be empty.");
        }

        private void ValidateLicenseType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new InvalidStaffAggregateException("LicenseType cannot be empty.");
        }

        private void ValidateIssuedBy(string issuedBy)
        {
            if (string.IsNullOrWhiteSpace(issuedBy))
                throw new InvalidStaffAggregateException("IssuedBy cannot be empty.");
        }

        private void ValidateExpiryDate(DateTime issueDate, DateTime expiryDate)
        {
            if (issueDate > expiryDate)
                throw new InvalidStaffAggregateException("IssueDate must before the ExpiryDate.");
        }
        #endregion
    }
}
