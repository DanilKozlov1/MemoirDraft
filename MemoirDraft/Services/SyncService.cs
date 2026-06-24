using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services
{
    public class SyncService
    {
        private readonly ILogger<SyncService> _logger;

        private readonly IFileStorageService _fileService;
        private readonly INoteService _noteService;


        public SyncService(ILogger<SyncService> logger,
            IFileStorageService fileService, INoteService noteService)
        {
            _logger = logger;

            _fileService = fileService;
            _noteService = noteService;
        }


        public async Task<int> SyncFromFileOnlyToDatabaseAsync(int userId)
        {
            var fileNotes = await _fileService.LoadAllNotesFromModeAsync("FileOnly");
            var userFileNotes = fileNotes.Where(n => n.UserId == userId).ToList();

            if (userFileNotes.Count == 0)
            {
                _logger.LogInformation("Нет файлов для синхронизации пользователя {UserId}", userId);
                return 0;
            }

            var added = 0;

            foreach (var fileNote in userFileNotes)
            {
                var oldId = fileNote.Id;

                fileNote.Id = 0;

                await _noteService.CreateAsync(fileNote);
                var newId = fileNote.Id;

                await _fileService.MoveNoteFilesAsync(oldId, newId, "FileOnly", "DatabaseAndFile");

                added++;
                _logger.LogInformation("Заметка '{Title}' синхронизирована (Id {OldId} → {NewId})",
                    fileNote.Title, oldId, newId);
            }

            return added;
        }
    }
}