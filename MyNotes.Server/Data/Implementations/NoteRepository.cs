using Microsoft.EntityFrameworkCore;
using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Data.Utils;
using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Data.Implementations
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        private readonly MyNotesDbContext _context;

        public NoteRepository(MyNotesDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
        {
            return await _context.Notes.Where(n => n.AuthorId == userId).ToListAsync();
        }
    }
}
