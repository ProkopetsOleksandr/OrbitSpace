using OrbitSpace.Application.Models.Requests;
using OrbitSpace.Application.Models.Responses;

namespace OrbitSpace.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<OperationResult<RegisterResult>> RegisterAsync(RegisterRequest request);
    Task<OperationResult<LoginResult>> LoginAsync(LoginRequest request);
}