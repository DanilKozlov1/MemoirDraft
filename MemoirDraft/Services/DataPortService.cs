using MemoirDraft.Database.DTO;
using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace MemoirDraft.Services
{
    public class DataPortService : IDataPortService
    {
        private readonly ILogger<DataPortService> _logger;
        
        private readonly INoteService _noteService;


        public DataPortService(ILogger<DataPortService> logger, INoteService noteService)
        {
            _logger = logger;

            _noteService = noteService;
        }


        public async Task<Note?> ImportNoteAsync(string filePath, int userId, bool isTodo)
        {
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
            if (lines.Length == 0) 
                return null;

            var title = Path.GetFileNameWithoutExtension(filePath);
            var note = new Note
            {
                UserId = userId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (isTodo)
            {
                note.NoteTypeId = 2;
                note.TodoItems = lines
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select((l, i) => new TodoItem { Id = i + 1, Text = l.Trim(), IsDone = false })
                    .ToList();
                note.Content = null;
            }
            else
            {
                note.NoteTypeId = 1;
                note.Content = string.Join(Environment.NewLine, lines);
                note.TodoItems = null;
            }
            return note;
        }
    }
}