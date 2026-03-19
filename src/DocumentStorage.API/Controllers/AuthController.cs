using Microsoft.AspNetCore.Mvc;
using DocumentStorage.Shared.DTOs.Auth;
using DocumentStorage.BusinessLayer.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DocumentStorage.Data.Repositories.Interfaces;

namespace DocumentStorage.API.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            return Ok(result);
        }

        [HttpGet("current-account")]
        [Authorize]
        public async Task<IActionResult> GetCurrentAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var accountId = await _authService.GetUserAccountIdAsync(userId);
            if (accountId == null)
            {
                return NotFound("Account not found for user");
            }

            return Ok(accountId);
        }
    }
}