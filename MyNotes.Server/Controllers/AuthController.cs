using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Login([FromBody] LoginRequest model)
        {
            // Validate credentials, e.g. from database
            if (model.Email == "1" && model.Password == "1")
            {
                var token = _jwtService.GenerateToken(model.Email);
                return Ok(new { token });
            }
            return Unauthorized("Invalid email or password");
        }


        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "19935988541-uftril2pfdatkoij0o5vu56t7j5e6ttp.apps.googleusercontent.com" }
                });

            if (payload == null) return Unauthorized();

            // Find or create user
            var user = await _userService.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    Username = payload.Name,
                    PasswordHash = "", // not needed for Google users
                    CreatedAt = DateTime.UtcNow
                };
                await _userService.CreateAsync(user);
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
}

