using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<OperationResult> RegisterAsync(RegisterRequestDto request);
        Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request, Guid userId);
        Task RevokeFamilyAsync(Guid familyId, TokenRevokedReason revokedReason);
        Task RevokeTokenAsync(RevokeRequestDto request);
        Task RevokeAllForUserAsync(Guid userId, TokenRevokedReason tokenRevokedReason);
    }
}