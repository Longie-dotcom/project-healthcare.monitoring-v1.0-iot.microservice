using Domain.Aggregate;
using Domain.DomainException;

namespace Domain.Entity
{
    public class PatientBedAssignment
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid PatientBedAssignmentID { get; private set; }
        public Guid PatientID { get; private set; }
        public string ControllerKey { get; private set; }
        public DateTime AssignedAt { get; private set; }
        public DateTime? ReleasedAt { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Patient Patient { get; protected set; }
        #endregion

        protected PatientBedAssignment() { }

        public PatientBedAssignment(
            Guid patientBedAssignmentID,
            Guid patientID,
            string controllerKey,
            DateTime assignedAt,
            DateTime? releasedAt = null)
        {
            ValidatePatientBedAssignmentID(patientBedAssignmentID);
            ValidatePatientID(patientID);
            ValidateControllerKey(controllerKey);
            ValidateAssignedAt(assignedAt);
            ValidateReleasedAt(assignedAt, releasedAt);

            PatientBedAssignmentID = patientBedAssignmentID;
            PatientID = patientID;
            ControllerKey = controllerKey;
            AssignedAt = assignedAt;
            ReleasedAt = releasedAt;
            IsActive = releasedAt == null;
        }

        #region Methods
        public void ReleaseBed(DateTime releasedAt)
        {
            ValidateReleasedAt(AssignedAt, releasedAt);
            ReleasedAt = releasedAt;
            IsActive = false;
        }

        public void ReassignBed(DateTime newAssignedAt)
        {
            if (IsActive)
                throw new InvalidPatientAggregateException(
                    "Cannot reassign an active bed without releasing first.");

            AssignedAt = newAssignedAt;
            ReleasedAt = null;
            IsActive = true;
        }
        #endregion

        #region Validators
        private void ValidatePatientBedAssignmentID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidPatientAggregateException("PatientBedAssignmentID cannot be empty.");
        }

        private void ValidatePatientID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidPatientAggregateException("PatientID cannot be empty.");
        }

        private void ValidateControllerKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidPatientAggregateException("ControllerKey cannot be empty.");
        }

        private void ValidateAssignedAt(DateTime assignedAt)
        {
            if (assignedAt == default)
                throw new InvalidPatientAggregateException("AssignedAt cannot be empty.");
        }

        private void ValidateReleasedAt(DateTime assignedAt, DateTime? releasedAt)
        {
            if (releasedAt.HasValue && releasedAt < assignedAt)
                throw new InvalidPatientAggregateException("ReleasedAt cannot be before AssignedAt.");
        }
        #endregion
    }
}
