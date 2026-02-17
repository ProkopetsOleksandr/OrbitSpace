using Microsoft.EntityFrameworkCore;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
