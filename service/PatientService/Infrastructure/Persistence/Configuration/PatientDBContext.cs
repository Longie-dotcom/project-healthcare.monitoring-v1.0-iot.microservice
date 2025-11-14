using Domain.Aggregate;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configuration
{
    public class PatientDBContext : DbContext
    {
        public PatientDBContext(DbContextOptions<PatientDBContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientStatus> PatientStatuses { get; set; }
        public DbSet<PatientBedAssignment> PatientBedAssignments { get; set; }
        public DbSet<PatientStaffAssignment> PatientStaffAssignments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------
            // Patient Aggregate
            // --------------------------
            var patientEntity = modelBuilder.Entity<Patient>();
            patientEntity.HasKey(p => p.PatientID);
            patientEntity.Property(p => p.PatientCode)
                         .IsRequired()
                         .HasMaxLength(50);
            patientEntity.Property(p => p.PatientStatusCode)
                         .IsRequired()
                         .HasMaxLength(50);
            patientEntity.Property(p => p.AdmissionDate).IsRequired();
            patientEntity.Property(p => p.DischargeDate);
            patientEntity.Property(p => p.IsActive);

            patientEntity.Property(p => p.IdentityNumber)
                         .IsRequired()
                         .HasMaxLength(50);
            patientEntity.Property(p => p.Email);
            patientEntity.Property(p => p.FullName);
            patientEntity.Property(p => p.Dob);
            patientEntity.Property(p => p.Address);
            patientEntity.Property(p => p.Gender);
            patientEntity.Property(p => p.Phone);

            patientEntity
                .HasOne(p => p.PatientStatus)
                .WithMany()
                .HasForeignKey(p => p.PatientStatusCode)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // --------------------------
            // Bed Assignments
            // --------------------------
            patientEntity
                .HasMany(p => p.PatientBedAssignments)
                .WithOne(b => b.Patient)
                .HasForeignKey(b => b.PatientID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientBedAssignment>(bed =>
            {
                bed.HasKey(b => b.PatientBedAssignmentID);
                bed.Property(b => b.ControllerKey)
                   .IsRequired()
                   .HasMaxLength(50);
                bed.Property(b => b.AssignedAt).IsRequired();
                bed.Property(b => b.ReleasedAt);
                bed.Property(b => b.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);
            });

            patientEntity
                .Navigation(nameof(Patient.PatientBedAssignments))
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // --------------------------
            // Staff Assignments
            // --------------------------
            patientEntity
                .HasMany(p => p.PatientStaffAssignment)
                .WithOne(b => b.Patient)
                .HasForeignKey(b => b.PatientID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PatientStaffAssignment>(staff =>
            {
                staff.HasKey(s => s.PatientStaffAssignmentID);
                staff.Property(s => s.StaffIdentityNumber)
                     .IsRequired()
                     .HasMaxLength(50);
                staff.Property(s => s.AssignedAt).IsRequired();
                staff.Property(s => s.UnassignedAt);
                staff.Property(s => s.IsActive)
                     .IsRequired()
                     .HasDefaultValue(true);
            });

            patientEntity
                .Navigation(nameof(Patient.PatientStaffAssignment))
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // --------------------------
            // Patient Statuses
            // --------------------------
            var statusEntity = modelBuilder.Entity<PatientStatus>();
            statusEntity.HasKey(s => s.PatientStatusCode);
            statusEntity.Property(s => s.PatientStatusCode)
                        .IsRequired()
                        .HasMaxLength(50);
            statusEntity.Property(s => s.Name)
                        .IsRequired()
                        .HasMaxLength(100);
            statusEntity.Property(s => s.Description)
                        .HasMaxLength(500);

            // --------------------------
            // Audit Logs
            // --------------------------
            modelBuilder.Entity<AuditLog>(audit =>
            {
                audit.HasKey(a => a.AuditLogID);
                audit.Property(a => a.EntityName)
                     .IsRequired()
                     .HasMaxLength(100);
                audit.Property(a => a.Action)
                     .IsRequired()
                     .HasMaxLength(50);
                audit.Property(a => a.PerformedBy)
                     .IsRequired()
                     .HasMaxLength(100);
                audit.Property(a => a.Timestamp)
                     .IsRequired();
                audit.Property(a => a.OldValue);
                audit.Property(a => a.NewValue);
            });
        }
    }
}
