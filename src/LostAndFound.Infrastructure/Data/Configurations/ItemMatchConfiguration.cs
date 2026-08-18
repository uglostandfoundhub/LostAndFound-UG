using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class ItemMatchConfiguration : IEntityTypeConfiguration<ItemMatch>
{
    public void Configure(EntityTypeBuilder<ItemMatch> builder)
    {
        builder.HasOne(m => m.LostItem)
            .WithMany()
            .HasForeignKey(m => m.LostItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.FoundItem)
            .WithMany()
            .HasForeignKey(m => m.FoundItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.LostItemId, m.FoundItemId }).IsUnique();
        builder.HasIndex(m => m.Score);
    }
}
