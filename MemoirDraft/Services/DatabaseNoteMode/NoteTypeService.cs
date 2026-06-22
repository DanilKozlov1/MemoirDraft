using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services.DatabaseNoteMode
{
    public class NoteTypeService : INoteTypeService
    {
        private readonly ILogger<NoteTypeService> _logger;
        private readonly INoteTypeRepository _noteTypeRepository;


        public NoteTypeService(ILogger<NoteTypeService> logger, INoteTypeRepository noteTypeRepository)
        {
            _logger = logger;
            _noteTypeRepository = noteTypeRepository;
        }


        public async Task<NoteType?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос на получение типа заметок по id={noteTypeId}.", id);
            try
            {
                NoteType? noteType = await _noteTypeRepository.GetByIdAsync(id);
                if (noteType == null)
                    _logger.LogWarning("Тип заметок по данному id({noteTypeId}) не найден или не существует.", id);

                return noteType;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<NoteType?> GetByNameAsync(string name)
        {
            _logger.LogInformation("Запрос на получение типа заметок по Name={noteTypeName}.", name);
            try
            {
                NoteType? noteType = await _noteTypeRepository.GetByNameAsync(name);
                if (noteType == null)
                    _logger.LogWarning("Тип заметок по данному Name({noteTypeName}) не найден или не существует.", name);

                return noteType;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<List<NoteType>?> GetAllAsync()
        {
            _logger.LogInformation("Запрос на получение всех типов заметок.");
            try
            {
                var list = await _noteTypeRepository.GetAllAsync();
                if (list == null || list.Count == 0)
                    _logger.LogWarning("Таблица NoteTypes пуста.");
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }
    }
}