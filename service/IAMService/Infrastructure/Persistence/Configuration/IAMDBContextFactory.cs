using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Configuration
{
    public class IAMDBContextFactory : IDesignTimeDbContextFactory<IAMDBContext>
    {
        public IAMDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IAMDBContext>();

            optionsBuilder.UseSqlServer("Server=.;Database=IAMDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new IAMDBContext(optionsBuilder.Options);
        }
    }
}