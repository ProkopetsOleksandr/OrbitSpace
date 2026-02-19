using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.TokenHash).HasMaxLength(64); // SHA-256
        builder.Property(m => m.DeviceInfo).HasMaxLength(256);
        builder.Property(m => m.CreatedAtUtc).HasColumnName("created_at");
        builder.Property(m => m.ExpiresAtUtc).HasColumnName("expires_at");
        builder.Property(m => m.RevokedAtUtc).HasColumnName("revoked_at");
        builder.Property(m => m.UsedAtUtc).HasColumnName("used_at");
        builder.Property(m => m.AbsoluteExpiresAtUtc).HasColumnName("absolute_expires_at");

        builder.HasIndex(m => m.TokenHash);
        builder.HasIndex(m => m.FamilyId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
