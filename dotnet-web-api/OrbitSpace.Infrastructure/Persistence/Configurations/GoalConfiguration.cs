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
        builder.Property(m => m.CreatedAtUtc).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(m => m.StartedAtUtc).HasColumnName("started_at");
        builder.Property(m => m.CompletedAtUtc).HasColumnName("completed_at");
        builder.Property(m => m.CanceledAtUtc).HasColumnName("canceled_at");
        builder.Property(g => g.Description).HasMaxLength(4000);
        builder.Property(g => g.Metrics).HasMaxLength(2000);
        builder.Property(g => g.AchievabilityRationale).HasMaxLength(2000);
        builder.Property(g => g.Motivation).HasMaxLength(2000);
        builder.Property(m => m.DueAtUtc).HasColumnName("due_at");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
