using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace MemoirDraft.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ILogger<FileStorageService> _logger;

        private JsonSerializerOptions _jsonOptions;

        private readonly string _baseDirectory;
        private readonly string _mode;


        public FileStorageService(ILogger<FileStorageService> logger, IConfiguration config)
        {
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions 
            { 
                WriteIndented = true
            };

            var notesPath = config["Storage:NotesPath"] ?? "Notes";
            _mode = config.GetValue<string>("Storage:Mode") ?? "DatabaseAndFile";
           
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, notesPath);
            var modeDir = Path.Combine(_baseDirectory, _mode);

            Directory.CreateDirectory(modeDir);
        }


        private string GetModeDirectory()
        {
            return Path.Combine(_baseDirectory, _mode);
        }

        public async Task SaveNoteFilesAsync(Note note)
        {
            try
            {
                var modeDir = GetModeDirectory();
                var safeTitle = string.Join("_", note.Title.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{note.Id}_{safeTitle}_{note.CreatedAt:yyyyMMdd_HHmmss}";
                var basePath = Path.Combine(modeDir, fileName);

                var jsonPath = basePath + ".json";
                //var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(note, _jsonOptions);

                await File.WriteAllTextAsync(jsonPath, json);
                _logger.LogInformation("Сохранён JSON для заметки {NoteId} в {JsonPath}", note.Id, jsonPath);

                if (note.NoteTypeId == 1)
                {
                    var txtPath = basePath + ".txt";

                    await File.WriteAllTextAsync(txtPath, note.Content ?? string.Empty);

                    _logger.LogInformation("Сохранена обычная заметка {NoteId} в {TxtPath}", note.Id, txtPath);
                }
                else if (note.NoteTypeId == 2)
                {
                    var txtPath = basePath + ".txt";
                    var txtContent = $"Заголовок: {note.Title}\n\nСписок задач:\n";

                    if (note.TodoItems != null && note.TodoItems.Any())
                    {
                        foreach (var item in note.TodoItems)
                            txtContent += $"  [{(item.IsDone ? "x" : " ")}] {item.Text}\n";
                    }
                    else
                        txtContent += "  (нет задач)";

                    await File.WriteAllTextAsync(txtPath, txtContent);
                    _logger.LogInformation("Сохранена TODO-заметка {NoteId} в {TxtPath}", note.Id, txtPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении файлов заметки {NoteId}", note.Id);
                throw;
            }
        }

        public async Task UpdateNoteFilesAsync(Note note)
        {
            try
            {
                await DeleteNoteFilesAsync(note.Id);
                await SaveNoteFilesAsync(note);

                _logger.LogInformation("Файлы для заметки {NoteId} обновлены", note.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении файлов заметки {NoteId}", note.Id);
                throw;
            }
        }

        public async Task DeleteNoteFilesAsync(int noteId)
        {
            try
            {
                var modeDir = GetModeDirectory();
                var files = Directory.GetFiles(modeDir, $"{noteId}_*");

                foreach (var file in files)
                {
                    File.Delete(file);
                    _logger.LogInformation("Удалён файл {File} для заметки {NoteId}", file, noteId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файлов заметки {NoteId}", noteId);
            }
        }

        public async Task<List<Note>> LoadAllNotesAsync()
        {
            var modeDir = GetModeDirectory();
            var notes = new List<Note>();
            if (!Directory.Exists(modeDir))
                return notes;

            var files = Directory.GetFiles(modeDir, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var note = JsonSerializer.Deserialize<Note>(json);
                    if (note != null)
                        notes.Add(note);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось прочитать файл {File}", file);
                }
            }
            return notes;
        }
    }
}