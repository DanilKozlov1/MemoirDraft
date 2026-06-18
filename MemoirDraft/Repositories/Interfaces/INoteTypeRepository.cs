using MemoirDraft.Database.Models;

namespace MemoirDraft.Repositories.Interfaces
{
    public interface INoteTypeRepository
    {
        Task<NoteType?> GetByIdAsync(int id);
        Task<NoteType?> GetByNameAsync(string name);
        Task<List<NoteType>> GetAllAsync();
    }
}