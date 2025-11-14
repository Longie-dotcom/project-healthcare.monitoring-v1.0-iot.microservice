namespace Application.DTO
{
    public class PatientStatusDTO
    {
        public string PatientStatusCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class PatientStatusCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string PatientStatusCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PatientStatusUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PatientStatusDeleteDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
    }
}
