namespace OrbitSpace.Application.Dtos.Authentication
{
    public record RegisterRequestDto(string Email, string FirstName, string LastName, string Password, DateOnly DateOfBirth);
}