namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IFrontendUrlBuilder
    {
        string BuildEmailVerificationUrl(string token);
    }
}
