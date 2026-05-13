using MyNotes.Server.Data.Utils;
using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Data.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId);
    }
}
