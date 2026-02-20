namespace OrbitSpace.Application.Common.Configuration;

public class FrontendOptions
{
    public const string SectionName = "Frontend";
    
    public required string BaseUrl { get; init; }
    public required string EmailVerificationUrlTemplate { get; init; }
}