using MemoirDraft.Database.Models;

namespace MemoirDraft.Services.Interfaces
{
    public interface INoteTypeService
    {
        Task<NoteType?> GetByIdAsync(int id);
        Task<NoteType?> GetByNameAsync(string name);
        Task<List<NoteType>?> GetAllAsync();
    }
}