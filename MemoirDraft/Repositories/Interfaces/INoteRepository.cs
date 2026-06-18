using MemoirDraft.Database.Models;

namespace MemoirDraft.Repositories.Interfaces
{
    public interface INoteRepository
    {
        Task<Note?> GetByIdAsync(int id);
        Task<Note?> GetByTitleAsync(string title);
        Task<List<Note>> GetAllByUserAsync(int userId);
        Task<List<Note>> GetAllByNoteTypeAsync(int userId, int noteTypeId);
        Task<List<Note>> GetFavoriteNotesAsync(int userId);
        Task AddAsync(Note note);
        Task<bool> UpdateAsync(Note note);
        Task<bool> DeleteAsync(int id);
    }
}