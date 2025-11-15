using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entity
{
    public class PatientStaff
    {
        #region Attributes
        #endregion

        #region Properties
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid PatientStaffID { get; private set; }
        public string PatientIdentityNumber { get; private set; } = string.Empty;
        public string StaffIdentityNumber { get; private set; } = string.Empty;
        public DateTime AssignedAt { get; private set; }
        public DateTime? UnassignedAt { get; private set; }
        public bool IsActive { get; private set; }

        public bool HasAccess
        {
            get { return IsActive && !IsUnassigned(); }
        }
        #endregion

        public PatientStaff() { }

        public PatientStaff(
            Guid patientStaffId,
            string patientIdentityNumber,
            string staffIdentityNumber, 
            DateTime assignedAt)
        {
            PatientStaffID = patientStaffId;
            PatientIdentityNumber = patientIdentityNumber;
            StaffIdentityNumber = staffIdentityNumber;
            AssignedAt = assignedAt;
            IsActive = true;
        }

        #region Methods
        public void UpdateStaffInfo(
            bool? isActive)
        {
            if (IsUnassigned())
                return;

            if (isActive.HasValue)
                IsActive = isActive.Value;
        }

        public void Unassign()
        {
            if (IsUnassigned())
                return;

            UnassignedAt = DateTime.UtcNow;
            IsActive = false;
        }

        private bool IsUnassigned()
        {
            return UnassignedAt != null;
        }
        #endregion
    }
}
