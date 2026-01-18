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
    }
}
