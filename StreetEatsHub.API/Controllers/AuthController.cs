using Microsoft.AspNetCore.Mvc;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Services;

namespace StreetEatsHub.API.Controllers
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

        /// <summary>
        /// Register a new vendor
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVendorDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterVendorAsync(registerDto);

            if (result == null)
            {
                return BadRequest(new { message = "Registration failed. Email may already be in use." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Login vendor
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Test endpoint to verify authentication
        /// </summary>
        [HttpGet("test")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Test()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var vendorId = User.FindFirst("VendorId")?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                message = "Authentication successful",
                userId,
                vendorId,
                email
            });
        }
    }
}