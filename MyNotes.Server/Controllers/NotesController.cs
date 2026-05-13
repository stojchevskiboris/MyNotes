using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("GetNotesByUserId")]
        public async Task<IActionResult> GetNotesByUserId(RequestIdModel model)
        {
            if (model.Id == 0)
            {
                return Unauthorized("User not found");
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
    }
}
