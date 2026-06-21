using MemoirDraft.Database.Models;

namespace MemoirDraft.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task SaveNoteFilesAsync(Note note);
        Task UpdateNoteFilesAsync(Note note);
        Task DeleteNoteFilesAsync(int noteId);
    }
}