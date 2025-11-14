namespace Domain.Aggregate
{
    public class PatientStatus
    {
        #region Attributes
        #endregion

        #region Properties
        public string PatientStatusCode { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        #endregion

        protected PatientStatus() { }

        public PatientStatus(string patientStatusCode, string description, string name)
        {
            ValidateCode(patientStatusCode);
            ValidateName(name);
            ValidateDescription(description);

            PatientStatusCode = patientStatusCode;
            Description = description;
            Name = name;
        }

        #region Methods
        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }

        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
        }
        #endregion

        #region Validators
        private void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("PatientStatusCode cannot be empty.");
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
        }

        private void ValidateDescription(string description)
        {
            if (description?.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");
        }
        #endregion
    }
}
