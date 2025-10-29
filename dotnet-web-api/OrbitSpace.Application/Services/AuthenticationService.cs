using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Dtos;
using OrbitSpace.Application.Models.Requests;
using OrbitSpace.Application.Models.Responses;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Services
{
    public class AuthenticationService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasherService passwordHasherService) : IAuthenticationService
    {
        private const string InvalidUsernameOrPasswordMessage = "Invalid username or password.";
        
        public async Task<OperationResult> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.FirstName)
                || string.IsNullOrWhiteSpace(request.LastName)
                || string.IsNullOrWhiteSpace(request.Password)
                || request.Password.Length < 6)
            {
                return OperationResultError.Validation("Invalid username or password");
            }

            if (await userRepository.GetByEmailAsync(request.Email) != null)
            {
                return OperationResultError.Validation("Email already exists");
            }

            await userRepository.CreateAsync(new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHasherService.HashPassword(request.Password)
            });

            return OperationResult.Success();
        }

        public async Task<OperationResult<LoginResult>> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return OperationResultError.Validation(InvalidUsernameOrPasswordMessage);
            }
            
            var passwordHasherResult = passwordHasherService.VerifyPassword(user.PasswordHash, request.Password);
            if (!passwordHasherResult)
            {
                return OperationResultError.Validation(InvalidUsernameOrPasswordMessage);
            }

            return new LoginResult
            {
                AccessToken = tokenService.GenerateAccessToken(user),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
        }
    }   
}