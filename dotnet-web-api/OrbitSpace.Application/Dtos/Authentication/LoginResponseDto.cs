namespace OrbitSpace.Application.Dtos.Authentication
{
    public record LoginResponseDto(string AccessToken, UserDto User);
}