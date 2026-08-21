using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.StudentId)
            .HasMaxLength(20);

        builder.Property(u => u.Department)
            .HasMaxLength(100);

        builder.HasIndex(u => u.StudentId)
            .IsUnique()
            .HasFilter("[StudentId] IS NOT NULL");
    }
}
