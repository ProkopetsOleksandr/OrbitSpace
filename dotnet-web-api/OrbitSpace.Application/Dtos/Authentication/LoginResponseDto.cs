namespace OrbitSpace.Application.Dtos.Authentication
{
    public record LoginResponseDto(string AccessToken, string RefreshToken, UserDto User);
}