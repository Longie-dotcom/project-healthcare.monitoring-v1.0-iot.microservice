using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Infrastructure.Persistence.Configuration
{
    public class StaffDBContextFactory : IDesignTimeDbContextFactory<StaffDBContext>
    {
        public StaffDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StaffDBContext>();

            // Use environment variable if available
            var connectionString = Environment.GetEnvironmentVariable("STAFF_DB_CONNECTION")
                                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=StaffDB;Trusted_Connection=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new StaffDBContext(optionsBuilder.Options);
        }
    }
}