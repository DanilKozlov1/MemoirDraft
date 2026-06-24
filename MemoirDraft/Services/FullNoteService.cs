using MemoirDraft.Database.Models;
using MemoirDraft.Services.DatabaseNoteMode;
using MemoirDraft.Services.FileOnlyNoteMode;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services
{
    public class FullNoteService : INoteService
    {
        private readonly ILogger<FullNoteService> _logger;

        private readonly INoteService _dbService;
        private readonly INoteService _fileService;


        public FullNoteService(ILogger<FullNoteService> logger,
            NoteService dbService, FileOnlyNoteService fileService)
        {
            _logger = logger;

            _dbService = dbService;
            _fileService = fileService;
        }


        public Task<Note?> GetByIdAsync(int id) => _dbService.GetByIdAsync(id);

        public Task<List<Note>?> GetAllByUserAsync(int userId) => _dbService.GetAllByUserAsync(userId);

        public Task<List<Note>?> GetAllByNoteTypeAsync(int userId, int noteTypeId) 
            => _dbService.GetAllByNoteTypeAsync(userId, noteTypeId);

        public Task<List<Note>?> GetFavoriteNotesAsync(int userId) => _dbService.GetFavoriteNotesAsync(userId);

        public async Task CreateAsync(Note note, bool storageMode=false)
        {
            await _dbService.CreateAsync(note);
            _logger.LogInformation("Заметка {NoteId} сохранена в БД", note.Id);

            try
            {
                await _fileService.CreateAsync(note, true);
                _logger.LogInformation("Файлы для заметки {NoteId} сохранены", note.Id);
            }
            catch
            {
                _logger.LogWarning("Ошибка сохранения файлов. Откат БД для заметки {NoteId}", note.Id);
                await _dbService.DeleteAsync(note.Id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            var dbResult = await _dbService.UpdateAsync(note);
            if (!dbResult) 
                return false;

            try
            {
                await _fileService.UpdateAsync(note);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении файлов. БД уже обновлена.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _fileService.DeleteAsync(id);
            var dbResult = await _dbService.DeleteAsync(id);

            return dbResult;
        }
    }
}