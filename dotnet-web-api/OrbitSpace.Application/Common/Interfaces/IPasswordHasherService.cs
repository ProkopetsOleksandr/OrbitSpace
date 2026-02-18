namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IPasswordHasherService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hash);
    }
}