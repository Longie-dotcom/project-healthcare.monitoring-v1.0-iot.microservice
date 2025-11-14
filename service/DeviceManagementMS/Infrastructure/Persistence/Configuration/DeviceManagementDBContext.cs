using Microsoft.EntityFrameworkCore;
using Domain.Aggregate;
using Domain.Entity;

namespace Infrastructure.Persistence.Configuration
{
    public class DeviceManagementDBContext : DbContext
    {
        public DeviceManagementDBContext(DbContextOptions<DeviceManagementDBContext> options)
            : base(options)
        {
        }

        // -------------------------------
        // DbSets
        // -------------------------------
        public DbSet<EdgeDevice> EdgeDevices { get; set; }
        public DbSet<Controller> Controllers { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //-----------------------------------------
            // EdgeDevice
            //-----------------------------------------
            modelBuilder.Entity<EdgeDevice>(entity =>
            {
                entity.ToTable("EdgeDevices");

                entity.HasKey(e => e.EdgeDeviceID);
                entity.Property(e => e.EdgeKey)
                    .IsRequired();
                entity.Property(e => e.RoomName)
                    .HasMaxLength(100);
                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);
                entity.Property(e => e.Description)
                    .HasMaxLength(255);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // Relationships
                entity.HasMany(e => e.Controllers)
                    .WithOne()
                    .HasForeignKey(c => c.EdgeKey)
                    .HasPrincipalKey(e => e.EdgeKey)
                    .OnDelete(DeleteBehavior.Cascade);

                // Field access for private collections
                entity.Navigation(e => e.Controllers)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            //-----------------------------------------
            // Controller
            //-----------------------------------------
            modelBuilder.Entity<Controller>(entity =>
            {
                entity.ToTable("Controllers");

                entity.HasKey(c => c.ControllerID);
                entity.Property(c => c.ControllerKey)
                    .IsRequired();
                entity.Property(c => c.EdgeKey);
                entity.Property(c => c.BedNumber)
                    .HasMaxLength(20);
                entity.Property(c => c.IpAddress);
                entity.Property(c => c.FirmwareVersion)
                    .HasMaxLength(50);
                entity.Property(c => c.Status)
                    .HasMaxLength(30);
                entity.Property(c => c.IsActive)
                    .HasDefaultValue(true);

                // Relationships
                entity.HasMany(c => c.Sensors)
                    .WithOne()
                    .HasForeignKey(s => s.ControllerKey)
                    .HasPrincipalKey(c => c.ControllerKey)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(c => c.Sensors)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            //-----------------------------------------
            // Sensor
            //-----------------------------------------
            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.ToTable("Sensors");

                entity.HasKey(s => s.SensorID);
                entity.Property(s => s.SensorKey)
                    .IsRequired();
                entity.Property(s => s.ControllerKey);
                entity.Property(s => s.Type)
                    .HasMaxLength(50);
                entity.Property(s => s.Unit)
                    .HasMaxLength(20);
                entity.Property(s => s.Description)
                    .HasMaxLength(255);
                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);
            });

            //-----------------------------------------
            // AuditLog
            //-----------------------------------------
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                entity.HasKey(a => a.AuditLogID);

                entity.Property(a => a.EntityName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(a => a.Action)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(a => a.PerformedBy)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(a => a.Timestamp)
                    .IsRequired();
                entity.Property(a => a.OldValue);
                entity.Property(a => a.NewValue);
            });
        }
    }
}
