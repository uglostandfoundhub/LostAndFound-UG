using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.VerificationQuestion)
            .HasMaxLength(300);

        builder.Property(i => i.ContactInfo)
            .HasMaxLength(200);

        builder.Property(i => i.Type)
            .HasConversion<int>();

        builder.Property(i => i.Status)
            .HasConversion<int>();

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Location)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ReportedBy)
            .WithMany(u => u.ReportedItems)
            .HasForeignKey(i => i.ReportedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.Type);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.DateOccurred);
        builder.HasIndex(i => new { i.Type, i.Status, i.CategoryId });
    }
}
