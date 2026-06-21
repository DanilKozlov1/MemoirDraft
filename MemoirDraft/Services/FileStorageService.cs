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

        private readonly string _notesDirectory;


        public FileStorageService(ILogger<FileStorageService> logger, IConfiguration config)
        {
            _logger = logger;

            var notesPath = config["FileStorage:NotesPath"] ?? "Notes";
            _notesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, notesPath);
            
            Directory.CreateDirectory(_notesDirectory);
        }


        public async Task SaveNoteFilesAsync(Note note)
        {
            try
            {
                var safeTitle = string.Join("_", note.Title.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{note.Id}_{safeTitle}_{note.CreatedAt:yyyyMMdd_HHmmss}";
                var basePath = Path.Combine(_notesDirectory, fileName);

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
                    {
                        txtContent += "  (нет задач)";
                    }

                    await File.WriteAllTextAsync(txtPath, txtContent);

                    var jsonPath = basePath + ".json";
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    var json = JsonSerializer.Serialize(note, jsonOptions);
                    
                    await File.WriteAllTextAsync(jsonPath, json);
                    _logger.LogInformation("Сохранена TODO-заметка {NoteId} в {TxtPath} и {JsonPath}", note.Id, txtPath, jsonPath);
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
                var files = Directory.GetFiles(_notesDirectory, $"{noteId}_*");
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
    }
}