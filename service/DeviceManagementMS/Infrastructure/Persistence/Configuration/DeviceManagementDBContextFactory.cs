using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Infrastructure.Persistence.Configuration
{
    public class DeviceManagementDBContextFactory : IDesignTimeDbContextFactory<DeviceManagementDBContext>
    {
        public DeviceManagementDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DeviceManagementDBContext>();

            // Use environment variable if available
            var connectionString = Environment.GetEnvironmentVariable("DEVICEMANAGEMENT_DB_CONNECTION")
                                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=DeviceManagementDB;Trusted_Connection=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new DeviceManagementDBContext(optionsBuilder.Options);
        }
    }
}