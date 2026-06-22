using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services.FileOnlyNoteMode
{
    public class FileOnlyNoteTypeService : INoteTypeService
    {
        private readonly ILogger<FileOnlyNoteTypeService> _logger;

        private readonly List<NoteType> _noteTypes;


        public FileOnlyNoteTypeService(ILogger<FileOnlyNoteTypeService> logger)
        {
            _logger = logger;

            _noteTypes = new()
            {
                new NoteType { Id = 1, Name = "simple" },
                new NoteType { Id = 2, Name = "todo" }
            };
        }


        public Task<NoteType?> GetByIdAsync(int id)
        {
            return Task.FromResult(_noteTypes.FirstOrDefault(t => t.Id == id));
        }

        public Task<NoteType?> GetByNameAsync(string name)
        {
            return Task.FromResult(_noteTypes.FirstOrDefault(t => t.Name == name));
        }

        public Task<List<NoteType>?> GetAllAsync()
        {
            return Task.FromResult<List<NoteType>?>(_noteTypes);
        }
    }
}