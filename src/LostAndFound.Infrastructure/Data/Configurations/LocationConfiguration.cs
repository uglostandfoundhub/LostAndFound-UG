using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(l => l.Building)
            .HasMaxLength(120);

        builder.HasIndex(l => l.Name);
    }
}
