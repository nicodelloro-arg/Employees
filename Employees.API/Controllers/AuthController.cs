using Employees.Data.Models;
using Employees.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Employees.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                    return BadRequest(new { message = "User and password required" });

                _logger.LogInformation("Attempting login for user: {Username}", request.Username);

                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    _logger.LogWarning("Login failed for user: {Username}", request.Username);
                    return Unauthorized(new { message = "Invalid login" });
                }

                _logger.LogInformation("Login successful for user: {Username}", request.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}