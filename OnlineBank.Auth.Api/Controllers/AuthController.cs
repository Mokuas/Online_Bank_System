using Microsoft.AspNetCore.Mvc;
using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineBank.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var email = User.FindFirstValue(ClaimTypes.Email)
                        ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new { userId, email, role });
        }
    }
}
