using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        
        builder.Property(g => g.Title).HasMaxLength(500);
        builder.Property(g => g.Description).HasMaxLength(4000);
        builder.Property(g => g.Metrics).HasMaxLength(2000);
        builder.Property(g => g.AchievabilityRationale).HasMaxLength(2000);
        builder.Property(g => g.Motivation).HasMaxLength(2000);

        builder.HasIndex(g => g.UserId);
    }
}
