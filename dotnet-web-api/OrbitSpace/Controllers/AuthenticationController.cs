using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.WebApi.Controllers
{
    
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ApiControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerDto)
        {
            var result = await authenticationService.RegisterAsync(registerDto);
            
            return !result.IsSuccess ? Problem() : Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await authenticationService.LoginAsync(email, password);
            
            return !result.IsSuccess ? Problem() : Ok(result.Data);
        }
    }
}
