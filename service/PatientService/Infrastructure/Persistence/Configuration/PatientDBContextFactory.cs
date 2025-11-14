using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Infrastructure.Persistence.Configuration
{
    public class PatientDBContextFactory : IDesignTimeDbContextFactory<PatientDBContext>
    {
        public PatientDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PatientDBContext>();

            // Use environment variable if available
            var connectionString = Environment.GetEnvironmentVariable("PATIENT_DB_CONNECTION")
                                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=PatientDB;Trusted_Connection=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new PatientDBContext(optionsBuilder.Options);
        }
    }
}