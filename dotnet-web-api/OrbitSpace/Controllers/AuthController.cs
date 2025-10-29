﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Requests;

namespace OrbitSpace.WebApi.Controllers
{
    
    [AllowAnonymous]
    public class AuthController(IAuthenticationService authenticationService) : ApiControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerDto)
        {
            var result = await authenticationService.RegisterAsync(registerDto);
            
            return !result.IsSuccess ? HandleFailure(result.Error) : Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await authenticationService.LoginAsync(request);
            
            return !result.IsSuccess ? HandleFailure(result.Error) : Ok(result.Data);
        }
    }
}
