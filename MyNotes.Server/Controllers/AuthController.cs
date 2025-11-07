using Microsoft.AspNetCore.Mvc;
using MyNotes.Server.Common.Helpers;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;
using MyNotes.Server.Services.ViewModels;

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

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await _jwtService.ValidateGoogleRequest(request);
            if (payload == null)
            {
                return Unauthorized();
            }

            var user = await _userService.CreateOrFindUser(payload);
            var jwtToken = _jwtService.GenerateToken(user.Email, user.Username, user.ProfileImageUrl);

            return Ok(new { token = jwtToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                await _userService.Login(model);
            }
            catch (ArgumentException argEx)
            {
                return Unauthorized(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }            

            var token = _jwtService.GenerateToken(model.Email);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            try
            {
                await _userService.Register(model);
            }
            catch (ArgumentException argEx)
            {
                return Unauthorized(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

            var token = _jwtService.GenerateToken(model.Email);
            return Ok(new { token });
        }
    }    
}

