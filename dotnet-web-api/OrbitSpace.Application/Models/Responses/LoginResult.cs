using OrbitSpace.Application.Models.Dtos;

namespace OrbitSpace.Application.Models.Responses;

public class LoginResult
{
    public required string AccessToken { get; set; }
    public required UserDto User { get; set; }
}