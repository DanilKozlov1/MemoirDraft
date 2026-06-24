using MemoirDraft.Database.Models;

namespace MemoirDraft.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<List<Note>> LoadAllNotesAsync();
        Task<List<Note>> LoadAllNotesFromModeAsync(string mode);
        Task SaveNoteFilesAsync(Note note);
        Task UpdateNoteFilesAsync(Note note);
        Task DeleteNoteFilesAsync(int noteId);
        Task MoveNoteFilesAsync(int oldNoteId, int newNoteId, string sourceMode, string targetMode);
    }
}