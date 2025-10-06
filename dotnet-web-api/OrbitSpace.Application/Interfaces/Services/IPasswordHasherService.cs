namespace OrbitSpace.Application.Interfaces.Services;

public interface IPasswordHasherService
{
    public string HashPassword(string password);
    public bool VerifyPassword(string hashedPassword, string providedPassword);
}