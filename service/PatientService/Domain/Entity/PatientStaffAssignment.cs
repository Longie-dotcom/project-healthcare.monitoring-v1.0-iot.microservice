using Domain.Aggregate;
using Domain.DomainException;

namespace Domain.Entity
{
    public class PatientStaffAssignment
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid PatientStaffAssignmentID { get; private set; }
        public Guid PatientID { get; private set; }
        public string StaffIdentityNumber { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public DateTime? UnassignedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Patient Patient { get; protected set; }
        #endregion

        protected PatientStaffAssignment() { }

        public PatientStaffAssignment(
            Guid patientStaffAssignmentId,
            Guid patientId,
            string staffIdentityNumber,
            DateTime assignedAt,
            bool isActive)
        {
            ValidatePatientStaffAssignmentID(patientStaffAssignmentId);
            ValidatePatientID(patientId);
            ValidateStaffIdentityNumber(staffIdentityNumber);
            ValidateAssignedAt(assignedAt);

            PatientStaffAssignmentID = patientStaffAssignmentId;
            PatientID = patientId;
            StaffIdentityNumber = staffIdentityNumber;
            AssignedAt = assignedAt;
            IsActive = isActive;
        }

        #region Methods
        public void Unassign(DateTime unassignedAt)
        {
            if (!IsActive)
                throw new InvalidPatientAggregateException(
                    "Staff assignment is already inactive.");

            if (unassignedAt < AssignedAt)
                throw new InvalidPatientAggregateException(
                    "UnassignedAt cannot be before AssignedAt.");

            UnassignedAt = unassignedAt;
            IsActive = false;
        }
        #endregion

        #region Validators
        private void ValidatePatientStaffAssignmentID(Guid patientStaffAssignmentId)
        {
            if (patientStaffAssignmentId == Guid.Empty)
                throw new InvalidPatientAggregateException("PatientStaffAssignmentId cannot be empty.");
        }

        private void ValidatePatientID(Guid patientId)
        {
            if (patientId == Guid.Empty)
                throw new InvalidPatientAggregateException("PatientID cannot be empty.");
        }

        private void ValidateStaffIdentityNumber(string staffIdentityNumber)
        {
            if (string.IsNullOrWhiteSpace(staffIdentityNumber))
                throw new InvalidPatientAggregateException("StaffIdentityNumber cannot be empty.");
        }

        private void ValidateAssignedAt(DateTime assignedAt)
        {
            if (assignedAt == default)
                throw new InvalidPatientAggregateException("AssignedAt cannot be empty.");
        }
        #endregion
    }
}
