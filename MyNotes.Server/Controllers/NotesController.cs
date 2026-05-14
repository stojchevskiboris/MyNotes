using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNotes.Server.Common.Middleware;
using MyNotes.Server.Common.Models;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;

namespace MyNotes.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
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

        [HttpPost("GetNotesByUserId")]
        public async Task<IActionResult> GetNotesByUserId(RequestIdModel model)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized("User not found");
            }

            if (model.Id != currentUserId)
            {
                return Unauthorized("You are not authorized to access these notes");
            }

            var userNotes = await _noteService.GetNotesByUserIdAsync(model.Id);

            var result = new
            {
                notes = userNotes,
                total = userNotes.Count()
            };

            return Ok(result);
        }

        [HttpPost("UpdateNote")]
        public IActionResult UpdateNote([FromBody] Note model)
        {
            var note = _noteService.GetNoteById(model.Id);
            if (note == null)
            {
                return NotFound("Note not found");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized("User not found");
            }

            if (note.AuthorId != currentUserId)
            {
                return Unauthorized("You are not authorized to update this note");
            }

            note.PosX = model.PosX;
            note.PosY = model.PosY;
            note.Width = model.Width;
            note.Height = model.Height;
            note.Title = model.Title;
            note.Content = model.Content;
            note.IsPinned = model.IsPinned;
            note.IsArchived = model.IsArchived;
            note.ColorTag = model.ColorTag;
            note.Tags = model.Tags;
            note.ModifiedAt = DateTime.UtcNow;

            _noteService.UpdateNote(note);

            return Ok(note);
        }

        [HttpPost("CreateNote")]
        public IActionResult CreateNote([FromBody] Note model)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized("User not found");
            }

            model.AuthorId = currentUserId;
            model.CreatedAt = DateTime.UtcNow;
            model.ModifiedAt = DateTime.UtcNow;

            _noteService.AddNote(model);

            return Ok(model);
        }

        [HttpPost("DeleteNote")]
        public IActionResult DeleteNote([FromBody] RequestIdModel model)
        {
            var note = _noteService.GetNoteById(model.Id);
            if (note == null)
            {
                return NotFound("Note not found");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized("User not found");
            }

            if (note.AuthorId != currentUserId)
            {
                return Unauthorized("You are not authorized to delete this note");
            }

            _noteService.DeleteNote(model.Id);

            return Ok();
        }
    }
}
