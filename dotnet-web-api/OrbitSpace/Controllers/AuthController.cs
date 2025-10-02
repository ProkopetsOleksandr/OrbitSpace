using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Interfaces.Repositories;
using OrbitSpace.Domain.Interfaces.Services;
using OrbitSpace.WebApi.Models;

namespace OrbitSpace.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _userRepository.GetByUsernameAsync(registerDto.Username) != null)
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerDto.Username
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, registerDto.Password);

            await _userRepository.CreateAsync(user);

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var passwordhasherResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (passwordhasherResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new AuthResponseDto
            {
                Username = user.Username,
                Token = token
            });
        }
    }
}
