using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Tags("Authentication")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ApiControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        [EndpointSummary("Register a new user")]
        [EndpointDescription("Creates a new user account with the provided email and password.")]
        [EndpointName("registerUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await authenticationService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Error.ErrorMessage);
            }

            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EndpointSummary("Login to get authentication tokens")]
        [EndpointDescription("Authenticates a user and returns access and refresh tokens.")]
        [EndpointName("loginUser")]
        [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await authenticationService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Authentication failed",
                    Detail = result.Error.ErrorMessage
                });
            }

            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [EndpointSummary("Refresh authentication tokens")]
        [EndpointDescription("Generates a new access token using a valid refresh token.")]
        [EndpointName("refreshToken")]
        [ProducesResponseType<RefreshResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var result = await authenticationService.RefreshAsync(request, CurrentUser.Id);

            if (!result.IsSuccess)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Token refresh failed",
                    Detail = result.Error.ErrorMessage
                });
            }

            return Ok(result.Data);
        }

        [HttpPost("revoke")]
        [AllowAnonymous]
        [EndpointSummary("Revoke a refresh token")]
        [EndpointDescription("Invalidates a specific refresh token, preventing further use.")]
        [EndpointName("revokeToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequestDto request)
        {
            await authenticationService.RevokeAsync(request);

            return Ok();
        }

        [HttpPost("revoke-all")]
        [EndpointSummary("Revoke all refresh tokens")]
        [EndpointDescription("Invalidates all refresh tokens for the currently authenticated user. Requires a valid access token.")]
        [EndpointName("revokeAllTokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeAll()
        {
            await authenticationService.RevokeAllAsync(CurrentUser.Id);

            return Ok();
        }
    }
}
