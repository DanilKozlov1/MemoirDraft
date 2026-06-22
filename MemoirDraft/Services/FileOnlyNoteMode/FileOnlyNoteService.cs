using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services.FileOnlyNoteMode
{
    public class FileOnlyNoteService : INoteService
    {
        private readonly ILogger<FileOnlyNoteService> _logger;
        
        private readonly IFileStorageService _fileStorage;
        private readonly INoteTypeService _noteTypeService;


        public FileOnlyNoteService(ILogger<FileOnlyNoteService> logger,
            IFileStorageService fileStorage, INoteTypeService noteTypeService)
        {
            _logger = logger;

            _fileStorage = fileStorage;
            _noteTypeService = noteTypeService;
        }


        private async Task SetNoteTypeAsync(Note note)
        {
            if (note == null) 
                return;

            var noteType = await _noteTypeService.GetByIdAsync(note.NoteTypeId);
            note.NoteType = noteType;
        }

        public async Task CreateAsync(Note note)
        {
            var allNotes = await _fileStorage.LoadAllNotesAsync();
            
            int newId = allNotes.Any() ? allNotes.Max(n => n.Id) + 1 : 1;
            note.Id = newId;

            await _fileStorage.SaveNoteFilesAsync(note);
            _logger.LogInformation("Заметка {NoteId} сохранена в файлы (режим FileOnly)", note.Id);
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            await _fileStorage.UpdateNoteFilesAsync(note);
            _logger.LogInformation("Заметка {NoteId} обновлена в файлах (режим FileOnly)", note.Id);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _fileStorage.DeleteNoteFilesAsync(id);
            _logger.LogInformation("Заметка {NoteId} удалена из файлов (режим FileOnly)", id);

            return true;
        }

        public async Task<List<Note>?> GetAllByUserAsync(int userId)
        {
            var notes = await _fileStorage.LoadAllNotesAsync();
            notes = notes.Where(n => n.UserId == userId).ToList();

            foreach (var note in notes)
                await SetNoteTypeAsync(note);
            
            return notes;
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            var notes = await _fileStorage.LoadAllNotesAsync();
            var note = notes.FirstOrDefault(n => n.Id == id);

            if (note != null)
                await SetNoteTypeAsync(note);
            
            return note;
        }

        public async Task<List<Note>?> GetAllByNoteTypeAsync(int userId, int noteTypeId)
        {
            var notes = await _fileStorage.LoadAllNotesAsync();

            notes = notes.Where(n => n.UserId == userId && n.NoteTypeId == noteTypeId).ToList();
            
            foreach (var note in notes)
                await SetNoteTypeAsync(note);
            
            return notes;
        }

        public async Task<List<Note>?> GetFavoriteNotesAsync(int userId)
        {
            var notes = await _fileStorage.LoadAllNotesAsync();
            notes = notes.Where(n => n.UserId == userId && n.IsFavorite).ToList();
            
            foreach (var note in notes)
                await SetNoteTypeAsync(note);
            
            return notes;
        }
    }
}