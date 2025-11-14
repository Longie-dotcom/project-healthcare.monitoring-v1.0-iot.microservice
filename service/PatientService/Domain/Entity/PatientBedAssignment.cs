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
            DateTime assignedAt)
        {
            ValidatePatientBedAssignmentID(patientBedAssignmentID);
            ValidatePatientID(patientID);
            ValidateControllerKey(controllerKey);
            ValidateAssignedAt(assignedAt);

            PatientBedAssignmentID = patientBedAssignmentID;
            PatientID = patientID;
            ControllerKey = controllerKey;
            AssignedAt = assignedAt;
            IsActive = true;
        }

        #region Methods
        public void ReleaseBed()
        {
            ReleasedAt = DateTime.UtcNow;
            IsActive = false;
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
        #endregion
    }
}
