using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required string TokenHash { get; set; }
    public Guid UserId { get; set; }
    public Guid FamilyId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public bool RememberMe { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public TokenRevokedReason? TokenRevokedReason { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public Guid? ReplacedByToken { get; set; } // for 30 sec grace period
    public string? DeviceInfo { get; set; }
    public DateTime AbsoluteExpiresAtUtc { get; set; }
    
    public bool IsActive => !RevokedAtUtc.HasValue
        && !UsedAtUtc.HasValue
        && ExpiresAtUtc > DateTime.UtcNow
        && AbsoluteExpiresAtUtc > DateTime.UtcNow;
}
