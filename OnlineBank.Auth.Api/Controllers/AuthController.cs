using Microsoft.AspNetCore.Mvc;
using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Services;

namespace OnlineBank.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            bool result = await _authService.RegisterAsync(request);
            if (!result)
                return BadRequest("Email already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            bool result = await _authService.LoginAsync(request);
            if (!result)
                return Unauthorized("Invalid email or password.");

            return Ok("Login successful.");
        }
    }
}
