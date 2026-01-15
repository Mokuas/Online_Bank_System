using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Security;
using OnlineBank.Auth.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlineBank.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, ICurrentUserService currentUser) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ICurrentUserService _currentUser = currentUser;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                userId = _currentUser.UserId,
                email = _currentUser.Email,
                role = _currentUser.Role
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (_currentUser.UserId is null)
                return Unauthorized(new { error = "Unauthorized." });

            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrWhiteSpace(jti))
                return BadRequest(new { error = "Token missing jti." });

            var expValue = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
            if (string.IsNullOrWhiteSpace(expValue) || !long.TryParse(expValue, out var expSeconds))
                return BadRequest(new { error = "Token missing exp." });

            var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

            // Revoke token
            var blacklist = HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();
            await blacklist.RevokeAsync(jti, _currentUser.UserId.Value, expiresAtUtc);

            return Ok(new { message = "Logged out." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _authService.GetUsersAsync();
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var result = await _authService.UpdateUserAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "User updated." });
        }

    }
}
