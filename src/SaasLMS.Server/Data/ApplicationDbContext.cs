using Microsoft.EntityFrameworkCore;
using SaasLMS.Shared.Models;

namespace SaasLMS.Server.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantService _tenantService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantService = tenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ProgressRecord> ProgressRecords => Set<ProgressRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure multi-tenant filters
        modelBuilder.Entity<Course>().HasQueryFilter(e => e.TenantId == _tenantService.CurrentTenant.Id);
        modelBuilder.Entity<User>().HasQueryFilter(e => e.TenantId == _tenantService.CurrentTenant.Id);

        // Configure relationships
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Modules)
            .WithOne()
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Module>()
            .HasMany(m => m.Contents)
            .WithOne()
            .HasForeignKey(c => c.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Enrollments)
            .WithOne()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasMany(e => e.ProgressRecords)
            .WithOne()
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Subdomain)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Email, u.TenantId })
            .IsUnique();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IHasTimestamps timestampEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        timestampEntity.CreatedAt = DateTime.UtcNow;
                        timestampEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        timestampEntity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            if (entry.Entity is IHasTenant tenantEntity && entry.State == EntityState.Added)
            {
                tenantEntity.TenantId = _tenantService.CurrentTenant.Id;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}