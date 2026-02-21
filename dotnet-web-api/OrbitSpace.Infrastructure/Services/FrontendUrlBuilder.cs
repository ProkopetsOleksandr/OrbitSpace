using Microsoft.Extensions.Options;
using OrbitSpace.Application.Common.Configuration;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Infrastructure.Services
{
    public class FrontendUrlBuilder(IOptions<FrontendOptions> frontendOptions) : IFrontendUrlBuilder
    {
        private readonly FrontendOptions _frontendOptions = frontendOptions.Value;

        public string BuildEmailVerificationUrl(string token)
        {
            var baseUrl = new Uri(_frontendOptions.BaseUrl);
            var emailVerificationPath = string.Format(_frontendOptions.EmailVerificationUrlTemplate, token);
            
            return new Uri(baseUrl, emailVerificationPath).ToString();
        }
    }
}
