using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using MyNotes.Server.Common;
using MyNotes.Server.Common.Helpers;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;

namespace MyNotes.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and password are required.");

            var user = await _userService.GetByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            if (user.IsGoogleUser)
                return Unauthorized("This account uses Google sign-in. Use the Google sign-in button.");

            var hashed = PasswordHelper.HashPassword(model.Password);
            if (!string.Equals(hashed, user.PasswordHash, StringComparison.Ordinal))
                return Unauthorized("Invalid email or password.");

            var token = _jwtService.GenerateToken(user.Email);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Username))
                return BadRequest("Username, email and password are required.");

            var existing = await _userService.GetByEmailAsync(model.Email);
            if (existing != null)
                return Conflict("A user with this email already exists.");

            if (!PasswordHelper.CheckPasswordStrength(model.Password))
                return BadRequest("Password does not meet strength requirements.");

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                AuthProvider = "Local",
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);

            var token = _jwtService.GenerateToken(user.Email);
            return Ok(new { token });
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { AppParameters.AppSettings.GoogleAuth.ClientId }
                });

            if (payload == null) return Unauthorized();

            // Find or create user
            var user = await _userService.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Username = payload.Name ?? payload.Email,
                    PasswordHash = null, // not needed for Google users
                    AuthProvider = AppParameters.AppSettings.GoogleAuth.AuthUri,
                    ProviderId = payload.Subject,
                    ProfileImageUrl = payload.Picture,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };
                await _userService.CreateAsync(user);
            }
            else
            {
                // Update metadata for existing user if needed
                user.AuthProvider ??= AppParameters.AppSettings.GoogleAuth.AuthUri;
                user.ProviderId ??= payload.Subject;
                user.ProfileImageUrl ??= payload.Picture;
                user.LastLoginAt = DateTime.UtcNow;
                // If your UserService exposes an update, call it. Repository update path is available via IUserRepository if needed.
            }

            // Generate your own JWT
            var jwt = _jwtService.GenerateToken(user.Email);

            return Ok(new { token = jwt });
        }
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}

