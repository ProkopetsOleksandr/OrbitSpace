namespace OrbitSpace.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? DeviceInfo { get; set; }

    // Computed properties
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsActive => !IsRevoked && !IsExpired && !UsedAtUtc.HasValue;

    // Navigation property
    public User User { get; set; } = null!;
}
