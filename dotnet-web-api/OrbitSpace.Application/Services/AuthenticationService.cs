using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Requests;
using OrbitSpace.Application.Models.Responses;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Services
{
    public class AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasherService passwordHasherService) : IAuthenticationService
    {
        private const string InvalidUsernameOrPasswordMessage = "Invalid username or password.";
        
        public async Task<OperationResult<RegisterResult>> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password)
                || request.Password.Length < 6)
            {
                return new OperationResult<RegisterResult>("Invalid username or password.");
            }

            if (await userRepository.GetByUsernameAsync(request.Username) != null)
            {
                return new OperationResult<RegisterResult>("User name already exists.");
            }

            await userRepository.CreateAsync(new User
            {
                Username = request.Username,
                PasswordHash = passwordHasherService.HashPassword(request.Password)
            });

            return new OperationResult<RegisterResult>(new RegisterResult
            {
                Message = "User registered successfully"
            });
        }

        public async Task<OperationResult<LoginResult>> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return new OperationResult<LoginResult>(InvalidUsernameOrPasswordMessage);
            }
            
            var passwordHasherResult = passwordHasherService.VerifyPassword(user.PasswordHash, request.Password);
            if (!passwordHasherResult)
            {
                return new OperationResult<LoginResult>(InvalidUsernameOrPasswordMessage);
            }
            
            return new OperationResult<LoginResult>(new LoginResult
            {
                Token = tokenService.GenerateToken(user)
            });
        }
    }   
}