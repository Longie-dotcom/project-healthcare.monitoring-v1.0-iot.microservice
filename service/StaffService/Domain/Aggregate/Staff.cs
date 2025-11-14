using Domain.DomainException;
using Domain.Entity;
using Domain.Enum;
using System.Text.RegularExpressions;

namespace Domain.Aggregate
{
    public class Staff
    {
        #region Attributes
        private readonly List<StaffLicense> staffLicenses = new();
        private readonly List<StaffSchedule> staffSchedules = new();
        private readonly List<StaffAssignment> staffAssignments = new();
        private readonly List<StaffExperience> staffExperiences = new();
        #endregion

        #region Properties
        public Guid StaffID { get; private set; }
        public string StaffCode { get; private set; }
        public string ProfessionalTitle { get; private set; }
        public string Specialization { get; private set; }
        public string AvatarUrl { get; private set; }
        public bool IsActive { get; private set; }

        public string IdentityNumber { get; private set; } // ↔ IAM.Users.IdentityNumber
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public DateTime Dob { get; private set; }
        public string Address { get; private set; }
        public string Gender { get; private set; }
        public string Phone { get; private set; }

        public IReadOnlyCollection<StaffLicense> StaffLicenses
        {
            get { return staffLicenses; }
        }
        public IReadOnlyCollection<StaffSchedule> StaffSchedules
        {
            get { return staffSchedules.AsReadOnly(); }
        }
        public IReadOnlyCollection<StaffAssignment> StaffAssignments
        {
            get { return staffAssignments.AsReadOnly(); }
        }
        public IReadOnlyCollection<StaffExperience> StaffExperiences
        {
            get { return staffExperiences.AsReadOnly(); }
        }
        #endregion

        protected Staff() { }

        public Staff(
            Guid staffId,
            string staffCode,
            string professionalTitle,
            string specialization,
            string avatarUrl,

            string identityNumber,
            string email,
            string fullName,
            DateTime dob,
            string address,
            string gender,
            string phone)
        {
            ValidateStaffID(staffId);
            ValidateStaffCode(staffCode);
            ValidateProfessionalTitle(professionalTitle);
            ValidateSpecialization(specialization);
            ValidateAvatarUrl(avatarUrl);

            StaffID = staffId;
            StaffCode = staffCode;
            ProfessionalTitle = professionalTitle;
            Specialization = specialization;
            AvatarUrl = avatarUrl;
            IsActive = true;

            IdentityNumber = identityNumber;
            Email = email;
            FullName = fullName;
            Dob = dob;
            Address = address;
            Gender = gender;
            Phone = phone;
        }

        #region Methods
        public void UpdateProfessionalTitle(string title)
        {
            ValidateProfessionalTitle(title);
            ProfessionalTitle = title;
        }

        public void UpdateSpecialization(string specialization)
        {
            ValidateSpecialization(specialization);
            Specialization = specialization;
        }

        public void UpdateAvatar(string avatarUrl)
        {
            ValidateAvatarUrl(avatarUrl);
            AvatarUrl = avatarUrl;
        }

        public void UpdateActive(bool active)
        {
            IsActive = active;
        }

        public void UpdateEmail(string email)
        {
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
            Phone = phone;
        }
        #endregion

        #region Methods - License
        public void AddLicense(string licenseNumber, string licenseType, string issuedBy, DateTime issueDate, DateTime expiryDate)
        {
            if (staffLicenses.Any(l => l.LicenseNumber == licenseNumber))
                throw new InvalidStaffAggregateException("License already exists.");

            var license = new StaffLicense(
                Guid.NewGuid(),
                this.StaffID,
                licenseNumber,
                licenseType,
                issuedBy,
                issueDate,
                expiryDate);

            staffLicenses.Add(license);
        }

        public void RemoveLicense(string licenseNumber)
        {
            var license = staffLicenses.FirstOrDefault(l => l.LicenseNumber == licenseNumber);
            if (license == null)
                throw new InvalidStaffAggregateException("License not found.");

            staffLicenses.Remove(license);
        }
        #endregion

        #region Methods - Schedule
        public void AddSchedule(string dayOfWeek, TimeSpan shiftStart, TimeSpan shiftEnd, bool isOnCall = false)
        {
            var schedule = new StaffSchedule(Guid.NewGuid(), this.StaffID, dayOfWeek, shiftStart, shiftEnd, isOnCall);
            staffSchedules.Add(schedule);
        }

        public void RemoveSchedule(Guid scheduleId)
        {
            var schedule = staffSchedules.FirstOrDefault(s => s.StaffScheduleID == scheduleId);
            if (schedule == null)
                throw new InvalidStaffAggregateException("Schedule not found.");

            staffSchedules.Remove(schedule);
        }
        #endregion

        #region Methods - Assignment
        public void AssignToDepartment(string department, string role, DateTime startDate)
        {
            var assignment = new StaffAssignment(Guid.NewGuid(), this.StaffID, department, role, startDate, null, true);
            staffAssignments.Add(assignment);
        }

        public void EndAssignment(Guid assignmentId, DateTime endDate)
        {
            var assignment = staffAssignments.FirstOrDefault(a => a.StaffAssignmentID == assignmentId);
            if (assignment == null)
                throw new InvalidStaffAggregateException("Assignment not found.");

            assignment.EndAssignment(endDate);
        }
        #endregion

        #region Methods - Experience
        public void AddExperience(string institution, string position, int startYear, int endYear, string description)
        {
            var exp = new StaffExperience(Guid.NewGuid(), this.StaffID, institution, position, startYear, endYear, description);
            staffExperiences.Add(exp);
        }

        public void RemoveExperience(Guid experienceId)
        {
            var exp = staffExperiences.FirstOrDefault(e => e.StaffExperienceID == experienceId);
            if (exp == null)
                throw new InvalidStaffAggregateException("Experience not found.");

            staffExperiences.Remove(exp);
        }
        #endregion

        #region Validators
        private void ValidateStaffID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffID cannot be empty.");
        }

        private void ValidateStaffCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidStaffAggregateException("StaffCode cannot be empty.");
        }

        private void ValidateProfessionalTitle(string professionalTitle)
        {
            if (string.IsNullOrWhiteSpace(professionalTitle))
                throw new InvalidStaffAggregateException("ProfessionalTitle cannot be empty.");
        }

        private void ValidateSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                throw new InvalidStaffAggregateException("Specialization cannot be empty.");
        }

        private void ValidateAvatarUrl(string avatarUrl)
        {

        }

        private static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidStaffAggregateException(
                    "Full name cannot be empty.");
        }

        private static void ValidateDob(DateTime dob)
        {
            if (dob > DateTime.Today)
                throw new InvalidStaffAggregateException(
                    "Date of birth cannot be in the future.");
        }

        private static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidStaffAggregateException(
                    "Address cannot be empty.");
        }

        private static void ValidateGender(string gender)
        {
            if (gender != GenderEnum.MALE && gender != GenderEnum.FEMALE)
                throw new InvalidStaffAggregateException(
                    "Gender must be male or female.");
        }
        #endregion
    }
}