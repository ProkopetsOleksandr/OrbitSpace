using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Authentication;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<OperationResult> RegisterAsync(RegisterRequestDto request);
        Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request, Guid userId);
        Task RevokeAsync(RevokeRequestDto request);
        Task RevokeAllAsync(Guid userId);
    }
}