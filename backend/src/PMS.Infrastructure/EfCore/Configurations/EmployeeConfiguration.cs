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

        // Value Objects (Owned types)
        builder.OwnsOne(e => e.FullName, fn =>
        {
            fn.Property(f => f.FirstName).HasColumnName("FirstName").HasMaxLength(100);
            fn.Property(f => f.LastName).HasColumnName("LastName").HasMaxLength(100);
            fn.Property(f => f.MiddleName).HasColumnName("MiddleName").HasMaxLength(100);
        });

        builder.OwnsOne(e => e.Email, e =>
        {
            e.Property(x => x.Value).HasColumnName("Email").HasMaxLength(200);
        });

        //Employee -> ManagedProjects (One-to-Many)
        builder.HasMany(e => e.ManagedProjects)
            .WithOne(p => p.Manager)
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        //Employee -> AssignedProjects (Many-to-Many)
        builder.HasMany(e => e.AssignedProjects)
            .WithMany(p => p.Employees)
            .UsingEntity<Dictionary<string, object>>(
                "EmployeeProject",
                j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),
                j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                j =>
                {
                    j.ToTable("EmployeeProjects");
                    j.HasKey("EmployeeId", "ProjectId");
                });
    }
}
