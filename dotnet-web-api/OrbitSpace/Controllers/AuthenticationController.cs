using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.WebApi.Controllers
{
    
    [ApiExplorerSettings(IgnoreApi = true)]
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
        public async Task<IActionResult> Login(LoginRequestDto loginDto)
        {
            var result = await authenticationService.LoginAsync(loginDto);

            return !result.IsSuccess ? Problem() : Ok(result.Data);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDto refreshDto)
        {
            var result = await authenticationService.RefreshAsync(refreshDto);

            return !result.IsSuccess ? Problem() : Ok(result.Data);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RevokeRequestDto revokeDto)
        {
            var result = await authenticationService.RevokeAsync(revokeDto);

            return !result.IsSuccess ? Problem() : Ok();
        }
    }
}
