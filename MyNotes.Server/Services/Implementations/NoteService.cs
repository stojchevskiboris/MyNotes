using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Domain.Models;
using MyNotes.Server.Services.Interfaces;

namespace MyNotes.Server.Services.Implementations
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
        {
            return await _noteRepository.GetNotesByUserIdAsync(userId);
        }

        public Note? GetNoteById(int id)
        {
            return _noteRepository.Get(id);
        }

        public void AddNote(Note note)
        {
            _noteRepository.Create(note);
        }

        public void UpdateNote(Note note)
        {
            _noteRepository.Update(note);
        }

        public void DeleteNote(int id)
        {
            _noteRepository.Delete(id);
        }
    }
}
