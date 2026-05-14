using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNotes.Server.Common.Middleware;
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

        private int GetCurrentUserId()
        {
            if (HttpContext.Items.TryGetValue(UserContextMiddleware.UserIdContextKey, out var contextUserId))
            {
                if (contextUserId is int userIdFromContext)
                {
                    return userIdFromContext;
                }
            }

            return 0;
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleUserLoginModel request)
        {
            var payload = await _jwtService.ValidateGoogleRequest(request);
            if (payload == null)
            {
                return Unauthorized();
            }

            var user = await _userService.CreateOrFindGoogleUser(payload);
            AttachUserIdToContext(user.Id);
            var result = _jwtService.GenerateTokenModel(user);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            UserViewModel user;
            try
            {
                user = await _userService.Login(model);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApplicationMessages.UnexpectedError);
            }

            AttachUserIdToContext(user.Id);
            var result = _jwtService.GenerateTokenModel(user);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
        {
            UserViewModel user;
            try
            {
                user = await _userService.Register(model);
            }
            catch (ArgumentException authEx)
            {
                return BadRequest(authEx.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, ApplicationMessages.UnexpectedError);
            }

            AttachUserIdToContext(user.Id);
            var result = _jwtService.GenerateTokenModel(user);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0 || currentUserId != model.Id)
            {
                return Unauthorized();
            }
            var user = await _userService.UpdateUser(model);
            return Ok(user);
        }

        private void AttachUserIdToContext(int userId)
        {
            if (userId > 0)
            {
                HttpContext.Items[UserContextMiddleware.UserIdContextKey] = userId;
            }
        }
    }    
}
