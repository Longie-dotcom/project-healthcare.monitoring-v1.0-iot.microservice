using Domain.Aggregate;
using Domain.Entity;
using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

public class IAMDBContext : DbContext
{
    public IAMDBContext(DbContextOptions<IAMDBContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Privilege> Privileges { get; set; }
    public DbSet<UserPrivilege> UserPrivileges { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePrivilege> RolePrivileges { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --------------------
        // User configuration
        // --------------------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserID);

            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.FullName).IsRequired();
            entity.Property(u => u.Dob).IsRequired();
            entity.Property(u => u.Address).IsRequired();
            entity.Property(u => u.Gender).IsRequired();
            entity.Property(u => u.Phone).IsRequired();
            entity.Property(u => u.IdentityNumber).IsRequired();
            entity.Property(u => u.Password)
                  .HasConversion(
                      v => v.Hashed,
                      v => Password.FromHash(v)
                  )
                  .IsRequired();
            entity.Property(u => u.IsActive).HasDefaultValue(true);

            entity.HasIndex(u => u.IdentityNumber);

            entity.HasMany(u => u.UserRoles)
                  .WithOne()
                  .HasForeignKey(ur => ur.UserID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.UserPrivileges)
                  .WithOne()
                  .HasForeignKey(ur => ur.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------
        // Role configuration
        // --------------------
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.RoleID);

            entity.Property(r => r.Name).IsRequired();
            entity.Property(r => r.Code).IsRequired();
            entity.Property(r => r.Description).IsRequired();

            entity.HasMany(r => r.RolePrivileges)
                  .WithOne()
                  .HasForeignKey(rp => rp.RoleID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------
        // Privilege configuration
        // --------------------
        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(p => p.PrivilegeID);

            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.Description).IsRequired();
        });

        // --------------------
        // UserRole configuration
        // --------------------
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => ur.UserRoleID);

            entity.Property(ur => ur.RoleID).IsRequired();
            entity.Property(ur => ur.UserID).IsRequired();
            entity.Property(ur => ur.IsActive).HasDefaultValue(true);

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles) // <- explicit navigation
                  .HasForeignKey(ur => ur.UserID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                  .WithMany() // no navigation on Role to avoid extra FK
                  .HasForeignKey(ur => ur.RoleID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------
        // UserPrivilege configuration
        // --------------------
        modelBuilder.Entity<UserPrivilege>(entity =>
        {
            entity.HasKey(up => up.UserPrivilegeID);

            entity.Property(up => up.PrivilegeID).IsRequired();
            entity.Property(up => up.UserID).IsRequired();
            entity.Property(up => up.IsGranted).HasDefaultValue(true);

            entity.HasOne(up => up.User)
                  .WithMany(u => u.UserPrivileges) // <- explicit navigation
                  .HasForeignKey(up => up.UserID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(up => up.Privilege)
                  .WithMany() // no navigation on Role to avoid extra FK
                  .HasForeignKey(up => up.PrivilegeID)
                  .OnDelete(DeleteBehavior.Restrict); // avoid multiple cascade paths
        });


        // --------------------
        // RolePrivilege configuration
        // --------------------
        modelBuilder.Entity<RolePrivilege>(entity =>
        {
            entity.HasKey(rp => rp.RolePrivilegeID);

            entity.Property(rp => rp.RoleID).IsRequired();
            entity.Property(rp => rp.PrivilegeID).IsRequired();
            entity.Property(rp => rp.IsActive).HasDefaultValue(true);

            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePrivileges)
                  .HasForeignKey(rp => rp.RoleID)
                  .OnDelete(DeleteBehavior.Cascade); // avoid multiple cascade paths

            entity.HasOne(rp => rp.Privilege)
                  .WithMany() // no navigation to Privilege collection
                  .HasForeignKey(rp => rp.PrivilegeID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --------------------
        // RefreshToken configuration
        // --------------------
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.RefreshTokenID);

            entity.Property(rt => rt.Token).IsRequired();
            entity.Property(rt => rt.UserID).IsRequired();
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);

            // One-to-one relation: User <-> RefreshToken
            entity.HasOne(rt => rt.User)
                  .WithOne(u => u.RefreshToken)
                  .HasForeignKey<RefreshToken>(rt => rt.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --------------------
        // AuditLog configuration
        // --------------------
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.AuditLogId);

            entity.Property(a => a.EntityName)
                  .IsRequired();

            entity.Property(a => a.Action)
                  .IsRequired();

            entity.Property(a => a.PerformedBy)
                  .HasMaxLength(100);

            entity.Property(a => a.OldValue);

            entity.Property(a => a.NewValue);

            entity.Property(a => a.Timestamp)
                  .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
