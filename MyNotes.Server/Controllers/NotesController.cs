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

        [HttpPost("GetNotesByUserId")]
        public async Task<IActionResult> GetNotesByUserId(RequestIdModel model)
       {
            if (model.Id == 0)
            {
                return Unauthorized("User not found");
            }

            var userNotes = DemoNotes;

            var result = new
            {
                notes = userNotes,
                total = userNotes.Count
            };

            return Ok(result);
        }

        private static readonly List<NoteDemoModel> DemoNotes = new()
        {
            new NoteDemoModel {
                Id = 1,
                Title = "Shopping List",
                Content = "Milk, eggs, bread, coffee",
                CreatedAt = DateTime.Now.AddDays(-10),
                ModifiedAt = DateTime.Now.AddDays(-8),
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = true,
                ColorTag = "#FFD700",
                Tags = new[] { "personal", "shopping" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 2,
                Title = "Angular TODOs",
                Content = "Refactor AuthService, fix Google login callback",
                CreatedAt = DateTime.Now.AddDays(-7),
                ModifiedAt = DateTime.Now.AddDays(-6),
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = false,
                ColorTag = "#ADD8E6",
                Tags = new[] { "work", "angular" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 3,
                Title = "Meeting Notes",
                Content = "Discuss deployment strategy with backend team.",
                CreatedAt = DateTime.Now.AddDays(-5),
                ModifiedAt = DateTime.Now.AddDays(-4),
                AuthorId = 102,
                AuthorUsername = "john",
                IsPinned = false,
                ColorTag = "#90EE90",
                Tags = new[] { "meeting", "team" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 4,
                Title = "Vacation Plan",
                Content = "Prespa lake weekend — book accommodation.",
                CreatedAt = DateTime.Now.AddDays(-4),
                ModifiedAt = DateTime.Now.AddDays(-4),
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = true,
                ColorTag = "#FFB6C1",
                Tags = new[] { "personal", "travel" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 5,
                Title = "MyNotes Ideas",
                Content = "Add dark mode, drag/drop, share notes feature.",
                CreatedAt = DateTime.Now.AddDays(-3),
                ModifiedAt = DateTime.Now.AddDays(-3),
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = true,
                ColorTag = "#FFA07A",
                Tags = new[] { "project", "ideas" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 6,
                Title = "Workout Routine",
                Content = "Mon: Chest, Tue: Back, Wed: Legs",
                CreatedAt = DateTime.Now.AddDays(-3),
                ModifiedAt = DateTime.Now.AddDays(-2),
                AuthorId = 103,
                AuthorUsername = "andrea",
                IsPinned = false,
                ColorTag = "#C0C0C0",
                Tags = new[] { "fitness" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 7,
                Title = "Learn .NET 8 Identity",
                Content = "Try external providers + custom JWT.",
                CreatedAt = DateTime.Now.AddDays(-1),
                ModifiedAt = DateTime.Now.AddHours(-12),
                AuthorId = 102,
                AuthorUsername = "john",
                IsPinned = false,
                ColorTag = "#6A5ACD",
                Tags = new[] { "learning", "backend" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 8,
                Title = "Quick Note",
                Content = "Test new app layout on mobile.",
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = false,
                ColorTag = "#F0E68C",
                Tags = new[] { "test", "mobile" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 9,
                Title = "Books to Read",
                Content = "Clean Code, The Pragmatic Programmer",
                CreatedAt = DateTime.Now.AddDays(-6),
                ModifiedAt = DateTime.Now.AddDays(-5),
                AuthorId = 103,
                AuthorUsername = "andrea",
                IsPinned = true,
                ColorTag = "#AFEEEE",
                Tags = new[] { "learning", "books" },
                IsArchived = false
            },
            new NoteDemoModel {
                Id = 10,
                Title = "Archived Notes",
                Content = "This note is archived but still visible to the user.",
                CreatedAt = DateTime.Now.AddDays(-20),
                ModifiedAt = DateTime.Now.AddDays(-18),
                AuthorId = 101,
                AuthorUsername = "boris",
                IsPinned = false,
                ColorTag = "#E6E6FA",
                Tags = new[] { "old" },
                IsArchived = true
            }
        };
    }
    public class NoteDemoModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = "";
        public bool IsPinned { get; set; }
        public string ColorTag { get; set; } = "";
        public string[] Tags { get; set; } = [];
        public bool IsArchived { get; set; }
    }
}
