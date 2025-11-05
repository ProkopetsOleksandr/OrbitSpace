using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Authentication;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<OperationResult> RegisterAsync(RegisterRequestDto request);
        Task<OperationResult<LoginResponseDto>> LoginAsync(string email, string password);
    }
}