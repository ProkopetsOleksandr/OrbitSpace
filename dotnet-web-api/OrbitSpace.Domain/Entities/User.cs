namespace OrbitSpace.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LockedUntilUtc { get; set; }
        public DateTime? TokensValidAfterUtc { get; set; }
    }
}
