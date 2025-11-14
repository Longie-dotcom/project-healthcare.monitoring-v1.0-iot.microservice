using Domain.Aggregate;
using Domain.DomainException;

namespace Domain.Entity
{
    public class StaffExperience
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid StaffExperienceID { get; private set; }
        public Guid StaffID { get; private set; }
        public string Institution { get; private set; }
        public string Position { get; private set; }
        public int StartYear { get; private set; }
        public int EndYear { get; private set; }
        public string Description { get; private set; }

        public Staff Staff { get; protected set; }
        #endregion

        protected StaffExperience() { }

        public StaffExperience(
            Guid staffExperienceID,
            Guid staffID,
            string institution,
            string position,
            int startYear,
            int endYear,
            string description)
        {
            ValidateStaffExperienceID(staffExperienceID);
            ValidateStaffID(staffID);
            ValidateInstitution(institution);
            ValidatePosition(position);
            ValidateYears(startYear, endYear);
            ValidateDescription(description);

            StaffExperienceID = staffExperienceID;
            StaffID = staffID;
            Institution = institution;
            Position = position;
            StartYear = startYear;
            EndYear = endYear;
            Description = description;
        }

        #region Methods
        public void UpdateInstitution(string institution)
        {
            ValidateInstitution(institution);
            Institution = institution;
        }

        public void UpdatePosition(string position)
        {
            ValidatePosition(position);
            Position = position;
        }

        public void UpdateYears(int start, int end)
        {
            ValidateYears(start, end);
            StartYear = start;
            EndYear = end;
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }
        #endregion

        #region Validators
        private void ValidateStaffExperienceID(Guid staffExperienceId)
        {
            if (staffExperienceId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffExperienceID cannot be empty.");
        }

        private void ValidateStaffID(Guid staffId)
        {
            if (staffId == Guid.Empty)
                throw new InvalidStaffAggregateException("StaffID cannot be empty.");
        }

        private void ValidateInstitution(string institution)
        {
            if (string.IsNullOrWhiteSpace(institution))
                throw new InvalidStaffAggregateException("Institution cannot be empty.");
        }

        private void ValidatePosition(string position)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new InvalidStaffAggregateException("Position cannot be empty.");
        }

        private void ValidateYears(int start, int end)
        {
            if (start < 1920 || start > DateTime.UtcNow.Year)
                throw new InvalidStaffAggregateException("Start year is out of valid range.");

            if (end < start)
                throw new InvalidStaffAggregateException("End year cannot be before start year.");
        }

        private void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidStaffAggregateException("Description cannot be empty.");
        }
        #endregion
    }
}
