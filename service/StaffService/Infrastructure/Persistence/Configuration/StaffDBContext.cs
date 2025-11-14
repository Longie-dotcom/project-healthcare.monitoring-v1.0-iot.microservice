using Microsoft.EntityFrameworkCore;
using Domain.Aggregate;
using Domain.Entity;

namespace Infrastructure.Persistence.Configuration
{
    public class StaffDBContext : DbContext
    {
        public StaffDBContext(DbContextOptions<StaffDBContext> options)
            : base(options)
        {
        }

        // -------------------------------
        // DbSets
        // -------------------------------
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<StaffLicense> StaffLicenses { get; set; }
        public DbSet<StaffAssignment> StaffAssignments { get; set; }
        public DbSet<StaffExperience> StaffExperiences { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }

        // -------------------------------
        // Configuration
        // -------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //-----------------------------------------
            // Staff
            //-----------------------------------------
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staffs");

                entity.HasKey(e => e.StaffID);
                entity.Property(e => e.StaffCode).IsRequired();
                entity.Property(e => e.ProfessionalTitle).HasMaxLength(100);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.AvatarUrl).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.Property(e => e.IdentityNumber).IsRequired();
                entity.Property(e => e.Email);
                entity.Property(e => e.FullName);
                entity.Property(e => e.Dob);
                entity.Property(e => e.Address);
                entity.Property(e => e.Gender);
                entity.Property(e => e.Phone);

                entity.HasIndex(e => e.StaffCode).IsUnique();
                entity.HasIndex(e => e.IdentityNumber).IsUnique();


                // Relationships
                entity.HasMany(e => e.StaffLicenses)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(l => l.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.StaffAssignments)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(a => a.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.StaffExperiences)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(ex => ex.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.StaffSchedules)
                    .WithOne(e => e.Staff)
                    .HasForeignKey(s => s.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(e => e.StaffLicenses)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Navigation(e => e.StaffSchedules)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Navigation(e => e.StaffAssignments)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.Navigation(e => e.StaffExperiences)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            //-----------------------------------------
            // StaffLicense
            //-----------------------------------------
            modelBuilder.Entity<StaffLicense>(entity =>
            {
                entity.ToTable("StaffLicenses");

                entity.HasKey(e => e.StaffLicenseID);

                entity.Property(e => e.LicenseNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.LicenseType).HasMaxLength(100);
                entity.Property(e => e.IssuedBy).HasMaxLength(100);
                entity.Property(e => e.IssueDate).HasColumnType("datetime");
                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
                entity.Property(e => e.IsValid).HasDefaultValue(true);

                entity.HasIndex(e => e.LicenseNumber).IsUnique();

                entity.HasOne(e => e.Staff)
                    .WithMany(s => s.StaffLicenses)
                    .HasForeignKey(e => e.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //-----------------------------------------
            // StaffAssignment
            //-----------------------------------------
            modelBuilder.Entity<StaffAssignment>(entity =>
            {
                entity.ToTable("StaffAssignments");

                entity.HasKey(e => e.StaffAssignmentID);

                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(e => e.Staff)
                    .WithMany(s => s.StaffAssignments)
                    .HasForeignKey(e => e.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //-----------------------------------------
            // StaffExperience
            //-----------------------------------------
            modelBuilder.Entity<StaffExperience>(entity =>
            {
                entity.ToTable("StaffExperiences");

                entity.HasKey(e => e.StaffExperienceID);

                entity.Property(e => e.Institution).HasMaxLength(200);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.StartYear);
                entity.Property(e => e.EndYear);
                entity.Property(e => e.Description).HasMaxLength(255);

                entity.HasOne(e => e.Staff)
                    .WithMany(s => s.StaffExperiences)
                    .HasForeignKey(e => e.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //-----------------------------------------
            // StaffSchedule
            //-----------------------------------------
            modelBuilder.Entity<StaffSchedule>(entity =>
            {
                entity.ToTable("StaffSchedules");

                entity.HasKey(e => e.StaffScheduleID);
                
                entity.Property(e => e.DayOfWeek)
                    .HasMaxLength(20);
                entity.Property(e => e.ShiftStart);
                entity.Property(e => e.ShiftEnd);
                entity.Property(e => e.IsActive).HasDefaultValue(true); // ✅ Available that day

                entity.HasOne(e => e.Staff)
                    .WithMany(s => s.StaffSchedules)
                    .HasForeignKey(e => e.StaffID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //-----------------------------------------
            // AuditLog
            //-----------------------------------------
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                entity.HasKey(e => e.AuditLogID);

                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.Action).HasMaxLength(50);
                entity.Property(e => e.PerformedBy).HasMaxLength(100);
                entity.Property(e => e.Timestamp);
                entity.Property(e => e.OldValue);
                entity.Property(e => e.NewValue);
            });
        }
    }
}
