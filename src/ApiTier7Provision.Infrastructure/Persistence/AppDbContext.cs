using ApiTier7Provision.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiTier7Provision.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HealthCheckLog> HealthCheckLogs => Set<HealthCheckLog>();

    public DbSet<ModelTemplate> ModelTemplates => Set<ModelTemplate>();

    public DbSet<ProvisionedInstance> ProvisionedInstances => Set<ProvisionedInstance>();

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

        modelBuilder.Entity<ModelTemplate>(entity =>
        {
            entity.ToTable("model_templates");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Model)
                .IsUnique();

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.Model)
                .HasColumnName("model")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(x => x.SupportedGpus)
                .HasColumnName("supported_gpus")
                .HasColumnType("text[]")
                .IsRequired();

            entity.Property(x => x.ProvisioningTemplateUuid)
                .HasColumnName("provisioning_template_uuid")
                .HasColumnType("text")
                .IsRequired();
        });

        modelBuilder.Entity<ProvisionedInstance>(entity =>
        {
            entity.ToTable("instances");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.VastInstanceId)
                .IsUnique();

            entity.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(x => x.ModelTemplateId)
                .HasColumnName("model_template_id")
                .IsRequired();

            entity.Property(x => x.VastInstanceId)
                .HasColumnName("vast_instance_id")
                .HasColumnType("bigint")
                .IsRequired();

            entity.Property(x => x.Gpu)
                .HasColumnName("gpu")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(x => x.TotalCostPerHour)
                .HasColumnName("total_cost_per_hour")
                .HasPrecision(18, 6)
                .IsRequired();

            entity.Property(x => x.IngressCost)
                .HasColumnName("ingress_cost")
                .HasPrecision(18, 6)
                .IsRequired();

            entity.Property(x => x.EgressCost)
                .HasColumnName("egress_cost")
                .HasPrecision(18, 6)
                .IsRequired();

            entity.Property(x => x.ProvisionedAtUtc)
                .HasColumnName("provisioned_at_utc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasOne(x => x.ModelTemplate)
                .WithMany(x => x.Instances)
                .HasForeignKey(x => x.ModelTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
