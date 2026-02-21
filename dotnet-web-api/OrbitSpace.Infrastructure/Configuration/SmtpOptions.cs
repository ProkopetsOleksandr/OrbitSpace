namespace OrbitSpace.Infrastructure.Configuration
{
    public class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public required string Host { get; init; }
        public required int Port { get; init; }
        public required string From { get; init; }
    }
}
