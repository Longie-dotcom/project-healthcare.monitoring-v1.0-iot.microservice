using Domain.DomainException;
using Domain.Entity;
using Domain.Enum;

namespace Domain.Aggregate
{
    public class Patient
    {
        #region Attributes
        private readonly List<PatientBedAssignment> patientBedAssignments = new();
        private readonly List<PatientStaffAssignment> patientStaffAssignment = new();
        #endregion

        #region Properties
        public Guid PatientID { get; private set; }
        public string PatientCode { get; private set; }
        public string PatientStatusCode { get; set; }
        public DateTime AdmissionDate { get; private set; }
        public DateTime? DischargeDate { get; private set; }
        public bool IsActive { get; private set; }

        public string IdentityNumber { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public DateTime Dob { get; private set; }
        public string Address { get; private set; }
        public string Gender { get; private set; }
        public string Phone { get; private set; }

        public PatientStatus PatientStatus { get; set; } = null!;
        public IReadOnlyCollection<PatientBedAssignment> PatientBedAssignments
        {
            get { return patientBedAssignments.AsReadOnly(); }
        }
        public IReadOnlyCollection<PatientStaffAssignment> PatientStaffAssignment
        {
            get { return patientStaffAssignment.AsReadOnly(); }
        }
        #endregion

        protected Patient() { }

        public Patient(
            Guid patientId, 
            string patientCode, 
            string patientStatusCode,
            DateTime admissionDate,

            string identityNumber,
            string email,
            string fullName,
            DateTime dob,
            string address,
            string gender,
            string phone)
        {
            ValidatePatientID(patientId);
            ValidatePatientCode(patientCode);
            ValidateStatus(patientStatusCode);
            ValidateAdmissionDate(admissionDate);

            PatientID = patientId;
            PatientCode = patientCode;
            PatientStatusCode = patientStatusCode;
            AdmissionDate = admissionDate;
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
        public void UpdateStatus(string status)
        {
            ValidateStatus(status);
            PatientStatusCode = status;
        }

        public void UpdateAdmissionDate(DateTime admissionDate)
        {
            ValidateAdmissionDate(admissionDate);
            AdmissionDate = admissionDate;
        }

        public void SetDischargeDate(DateTime dischargeDate)
        {
            ValidateDischargeDate(dischargeDate);
            DischargeDate = dischargeDate;
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

        #region Methods - Bed Assignment
        public PatientBedAssignment? AssignBed(PatientBedAssignment bedAssignment)
        {
            if (bedAssignment == null)
                throw new InvalidPatientAggregateException("BedAssignment cannot be null.");

            // Only one bed per patient
            var activeBed = patientBedAssignments
                .FirstOrDefault(b => b.ReleasedAt == null);
            if (activeBed != null)
            {
                activeBed.ReleaseBed();
                return activeBed; // Let service layer publish the unassignment
            }

            patientBedAssignments.Add(bedAssignment);
            return null;
        }

        public PatientBedAssignment ReleaseBed(Guid bedAssignmentID)
        {
            var bed = patientBedAssignments
                .FirstOrDefault(b => b.PatientBedAssignmentID == bedAssignmentID);
            if (bed == null) throw new InvalidPatientAggregateException(
                "BedAssignment not found.");

            bed.ReleaseBed();
            return bed; // Let service layer publish the unassignment
        }
        #endregion

        #region Methods - Staff Assignment
        public void AssignStaff(PatientStaffAssignment staffAssignment)
        {
            if (staffAssignment == null)
                throw new InvalidPatientAggregateException("StaffAssignment cannot be null.");

            // Prevent duplicate active assignments
            var activeStaff = patientStaffAssignment
                .FirstOrDefault(s => s.StaffIdentityNumber == staffAssignment.StaffIdentityNumber && s.UnassignedAt == null);
            if (activeStaff != null) 
                throw new InvalidPatientAggregateException("Staff has been assigned to this patient already");

            patientStaffAssignment.Add(staffAssignment);
        }

        public PatientStaffAssignment UnassignStaff(Guid staffAssignmentID, DateTime unassignedAt)
        {
            var assignment = patientStaffAssignment
                .FirstOrDefault(s => s.PatientStaffAssignmentID == staffAssignmentID && s.UnassignedAt == null);
            if (assignment == null)
                throw new InvalidPatientAggregateException(
                    "Active staff assignment not found.");

            assignment.Unassign();
            return assignment;
        }
        #endregion

        #region Validators
        private void ValidatePatientID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidPatientAggregateException("PatientID cannot be empty.");
        }

        private void ValidatePatientCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidPatientAggregateException("PatientCode cannot be empty.");
        }

        private void ValidateAdmissionDate(DateTime date)
        {
            if (date == default)
                throw new InvalidPatientAggregateException("AdmissionDate cannot be empty.");
        }

        private void ValidateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new InvalidPatientAggregateException("Status cannot be empty.");
        }

        private void ValidateDischargeDate(DateTime dischargeDate)
        {
            if (dischargeDate < AdmissionDate)
                throw new InvalidPatientAggregateException("DischargeDate cannot be before AdmissionDate.");
        }

        private static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidPatientAggregateException(
                    "Full name cannot be empty.");
        }

        private static void ValidateDob(DateTime dob)
        {
            if (dob > DateTime.Today)
                throw new InvalidPatientAggregateException(
                    "Date of birth cannot be in the future.");
        }

        private static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidPatientAggregateException(
                    "Address cannot be empty.");
        }

        private static void ValidateGender(string gender)
        {
            if (gender != GenderEnum.MALE && gender != GenderEnum.FEMALE)
                throw new InvalidPatientAggregateException(
                    "Gender must be male or female.");
        }
        #endregion
    }
}
