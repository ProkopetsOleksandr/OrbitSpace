using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Requests;

namespace OrbitSpace.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IAuthenticationService authenticationService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerDto)
        {
            var result = await authenticationService.RegisterAsync(registerDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await authenticationService.LoginAsync(request);
            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
