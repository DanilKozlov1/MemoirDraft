using MemoirDraft.Database.Models;

namespace MemoirDraft.Services.Interfaces
{
    public interface IDataPortService
    {
        Task<Note?> ImportNoteAsync(string filePath, int userId, bool isTodo);
    }
}