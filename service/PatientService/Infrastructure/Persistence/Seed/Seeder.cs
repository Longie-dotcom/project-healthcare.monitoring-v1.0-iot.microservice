using Application.Enum;
using Domain.Aggregate;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed
{
    public static class PatientDB
    {
        public static async Task Seed(PatientDBContext context)
        {
            if (context == null) return;

            // Ensure database is created
            await context.Database.MigrateAsync();

            // -------------------------------
            // Seed PatientStatuses
            // -------------------------------
            var statuses = new List<PatientStatus>
            {
                new PatientStatus(PatientStatusEnum.Admitted, "Patient is currently admitted", "Admitted"),
                new PatientStatus(PatientStatusEnum.Discharged, "Patient has been discharged", "Discharged"),
                new PatientStatus(PatientStatusEnum.Transferred, "Patient has been transferred to another facility", "Transferred"),
                new PatientStatus(PatientStatusEnum.PreAdmitted, "Patient identified and prepared for admission", "Pending admission confirmation")
            };
            
            foreach (var status in statuses)
            {
                var exists = await context.PatientStatuses
                                          .AsNoTracking()
                                          .AnyAsync(s => s.PatientStatusCode == status.PatientStatusCode);
                if (!exists)
                {
                    await context.PatientStatuses.AddAsync(status);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
