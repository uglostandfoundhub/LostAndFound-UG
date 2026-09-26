using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.Property(i => i.FilePath)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(i => i.FileName)
            .HasMaxLength(200);

        builder.HasOne(i => i.Item)
            .WithMany(i => i.Images)
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
