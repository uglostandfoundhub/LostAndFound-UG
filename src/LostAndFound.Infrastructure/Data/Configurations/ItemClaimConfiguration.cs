using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Data.Configurations;

public class ItemClaimConfiguration : IEntityTypeConfiguration<ItemClaim>
{
    public void Configure(EntityTypeBuilder<ItemClaim> builder)
    {
        builder.Property(c => c.VerificationAnswer)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.SupportingDetails)
            .HasMaxLength(1000);

        builder.Property(c => c.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(c => c.Status)
            .HasConversion<int>();

        builder.HasOne(c => c.Item)
            .WithMany(i => i.Claims)
            .HasForeignKey(c => c.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Claimant)
            .WithMany(u => u.SubmittedClaims)
            .HasForeignKey(c => c.ClaimantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ReviewedBy)
            .WithMany()
            .HasForeignKey(c => c.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.ItemId, c.ClaimantId }).IsUnique();
        builder.HasIndex(c => c.Status);
    }
}
