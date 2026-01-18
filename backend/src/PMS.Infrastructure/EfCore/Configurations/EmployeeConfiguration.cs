using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PMS.Domain.Entities;

namespace PMS.Infrastructure.EfCore.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        // Value Objects
        builder.OwnsOne(e => e.FullName, fn =>
        {
            fn.Property(f => f.FirstName).HasColumnName("FirstName").HasMaxLength(100);
            fn.Property(f => f.LastName).HasColumnName("LastName").HasMaxLength(100);
            fn.Property(f => f.MiddleName).HasColumnName("MiddleName").HasMaxLength(100);
        });

        builder.OwnsOne(e => e.Email, e =>
        {
            e.Property(x => x.Value)
                .HasColumnName("email")
                .HasMaxLength(100);
        });

        // Навигационные свойства
        builder.HasMany<Project>()
            .WithOne(p => p.Manager)
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.AssignedProjects)
            .WithMany(p => p.Employees)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeProjects",
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),
                j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                j => j.ToTable("EmployeeProjects"));
    }
}
