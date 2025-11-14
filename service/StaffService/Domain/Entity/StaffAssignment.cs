using Domain.Aggregate;
using Domain.DomainException;

namespace Domain.Entity
{
    public class StaffAssignment
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid StaffAssignmentID { get; private set; }
        public Guid StaffID { get; private set; }
        public string Department { get; private set; }
        public string Role { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsActive { get; private set; }

        public Staff Staff { get; protected set; }
        #endregion

        protected StaffAssignment() { }

        public StaffAssignment(
            Guid staffAssignmentID,
            Guid staffID,
            string department,
            string role,
            DateTime startDate,
            DateTime? endDate,
            bool isActive)
        {
            ValidateStaffAssignmentID(staffAssignmentID);
            ValidateStaffID(staffID);
            ValidateDepartment(department);
            ValidateRole(role);
            ValidateStartDate(startDate, endDate);

            StaffAssignmentID = staffAssignmentID;
            StaffID = staffID;
            Department = department;
            Role = role;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
        }

        #region Methods
        public void UpdateDepartment(string department)
        {
            ValidateDepartment(department);
            Department = department;
        }

        public void UpdateRole(string role) 
        { 
            ValidateRole(role); 
            Role = role;
        }

        public void UpdateStartDate(DateTime startDate, DateTime endDate)
        {
            ValidateStartDate(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;
        }

        public void EndAssignment(DateTime endDate)
        {
            ValidateStartDate(StartDate, endDate);
            EndDate = endDate;
            IsActive = false;
        }
        #endregion

        #region Validators
        private void ValidateStaffAssignmentID(Guid staffAssignmentId)
        {
            if (staffAssignmentId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffAssignmentID cannot be empty.");
        }

        private void ValidateStaffID(Guid staffId)
        {
            if (staffId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffID cannot be empty.");
        }

        private void ValidateDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new InvalidStaffAggregateException("Department cannot be empty.");
        }

        private void ValidateRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new InvalidStaffAggregateException("Role cannot be empty.");
        }

        private void ValidateStartDate(DateTime startDate, DateTime? endDate)
        {
            if (startDate > endDate)
                throw new InvalidStaffAggregateException("StartDate must be before EndDate.");
        }
        #endregion
    }
}
