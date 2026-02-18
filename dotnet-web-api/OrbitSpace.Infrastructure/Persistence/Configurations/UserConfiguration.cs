using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(u => u.Email).HasMaxLength(256); // Todo: .UseCollation("case_insensitive");
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasMaxLength(128);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
