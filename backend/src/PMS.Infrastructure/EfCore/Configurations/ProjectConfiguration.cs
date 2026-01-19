using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMS.Domain.Entities;

namespace PMS.Infrastructure.EfCore.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        // Свойства
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.CustomerCompanyName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.ExecutorCompanyName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.Priority)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    : v.ToUniversalTime(),
                v => v);

        builder.Property(p => p.EndDate)
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    : v.ToUniversalTime(),
                v => v);

        // Связь: Project -> Manager (One-to-Many)
        builder.HasOne(p => p.Manager)
            .WithMany(e => e.ManagedProjects)
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь: Project -> Employees (Many-to-Many)
        builder.HasMany(p => p.Employees)
            .WithMany(e => e.AssignedProjects)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeProject",
                j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),
                j =>
                {
                    j.ToTable("EmployeeProjects");
                    j.HasKey("EmployeeId", "ProjectId");
                });

        // Связь: Project -> Documents (One-to-Many)
        builder.HasMany(p => p.Documents)
            .WithOne(d => d.Project)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
