namespace OrbitSpace.Application.Dtos.Authentication;

public record LoginRequestDto(string Email, string Password, bool RememberMe, string? DeviceInfo);
