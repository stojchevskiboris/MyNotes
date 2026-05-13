using MyNotes.Server.Domain.Models;

namespace MyNotes.Server.Services.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId);
        Note? GetNoteById(int id);
        void AddNote(Note note);
        void UpdateNote(Note note);
        void DeleteNote(int id);
    }
}
