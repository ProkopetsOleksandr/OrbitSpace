using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.WebApi.Controllers
{
    /// <summary>
    /// Authentication endpoints for user registration, login, token refresh, and revocation
    /// </summary>
    [Route("api/[controller]")]
    [Tags("Authentication")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ApiControllerBase
    {
        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration details including email and password</param>
        /// <returns>Success response with no data on successful registration</returns>
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

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials (email and password)</param>
        /// <returns>Access token and refresh token on successful authentication</returns>
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

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New access token and refresh token pair</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [EndpointSummary("Refresh authentication tokens")]
        [EndpointDescription("Generates a new access token using a valid refresh token.")]
        [EndpointName("refreshTokens")]
        [ProducesResponseType<RefreshResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var result = await authenticationService.RefreshAsync(request);

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

        /// <summary>
        /// Revoke a specific refresh token
        /// </summary>
        /// <param name="request">Refresh token to revoke</param>
        /// <returns>Success response with no data</returns>
        [HttpPost("revoke")]
        [AllowAnonymous]
        [EndpointSummary("Revoke a refresh token")]
        [EndpointDescription("Invalidates a specific refresh token, preventing further use.")]
        [EndpointName("revokeToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequestDto request)
        {
            var result = await authenticationService.RevokeAsync(request);

            if (!result.IsSuccess)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Error.ErrorMessage);
            }

            return Ok();
        }

        /// <summary>
        /// Revoke all refresh tokens for the authenticated user
        /// </summary>
        /// <returns>Success response with no data</returns>
        [HttpPost("revoke-all")]
        [Authorize]
        [EndpointSummary("Revoke all refresh tokens")]
        [EndpointDescription("Invalidates all refresh tokens for the currently authenticated user. Requires a valid access token.")]
        [EndpointName("revokeAllTokens")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RevokeAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid user identity",
                    Detail = "Could not extract user ID from access token"
                });
            }

            var result = await authenticationService.RevokeAllAsync(userId);

            if (!result.IsSuccess)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Error.ErrorMessage);
            }

            return Ok();
        }
    }
}
