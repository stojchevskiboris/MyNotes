using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;

namespace MyNotes.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly INoteService _noteService;

        public NotesController(
            IUserService userService
            //INoteService noteService
            )
        {
            _userService = userService;
            //_noteService = noteService;
        }

        [HttpGet("my-notes")]
        public async Task<IActionResult> GetMyNotes()
        {
            // The user is attached by JWT Bearer middleware
            var user = HttpContext.Items["User"] as User;
            if (user == null)
            {
                return Unauthorized("User not found in context");
            }

            // Fetch notes for the authenticated user
            var notes = true;//await _noteRepository.GetNotesByUserIdAsync(user.Id);

            return Ok(notes);
        }
    }
}
