using ApiTier7Provision.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiTier7Provision.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HealthCheckLog> HealthCheckLogs => Set<HealthCheckLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HealthCheckLog>(entity =>
        {
            entity.ToTable("health_check_logs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.LoggedAtUtc)
                .HasColumnName("logged_at_utc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        });
    }
}
