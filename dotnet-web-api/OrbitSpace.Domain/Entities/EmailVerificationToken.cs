namespace OrbitSpace.Domain.Entities
{
    public class EmailVerificationToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string TokenHash { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsUsed { get; set; }
    }
}
