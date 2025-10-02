namespace OrbitSpace.Infrastructure.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public required string Key { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
    }
}
