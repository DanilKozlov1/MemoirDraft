using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services.DatabaseNoteMode
{
    public class NoteService : INoteService
    {
        private readonly ILogger<NoteService> _logger;

        private readonly INoteRepository _noteRepository;
        private readonly IUserRepository _userRepository;


        public NoteService(ILogger<NoteService> logger, 
            INoteRepository noteRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _noteRepository = noteRepository;
            _userRepository = userRepository;
        }


        public async Task<Note?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос на получение заметки по id={noteId}.", id);
            try
            {
                Note? note = await _noteRepository.GetByIdAsync(id);
                if (note == null)
                    _logger.LogWarning("Заметка по данному id({noteId}) не найден или не существует.", id);

                return note;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<List<Note>?> GetAllByUserAsync(int userId)
        {
            _logger.LogInformation("Запрос на получение всех заметок пользователя({userId}).", userId);
            try
            {
                if (await _userRepository.GetByIdAsync(userId) == null)
                {
                    _logger.LogWarning("Пользователя по Id={userId} не найдено или не существует.", userId);
                    return null;
                }

                var list = await _noteRepository.GetAllByUserAsync(userId);
                if (list == null || list.Count == 0)
                    _logger.LogInformation("У пользователя({userId}) нет заметок.", userId);

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<List<Note>?> GetAllByNoteTypeAsync(int userId, int noteTypeId)
        {
            _logger.LogInformation("Запрос на получение всех заметок пользователя({userId}) и типу({noteTypeId}).", 
                userId, noteTypeId);
            try
            {
                if (await _userRepository.GetByIdAsync(userId) == null)
                {
                    _logger.LogWarning("Пользователя по Id={userId} не найдено или не существует.", userId);
                    return null;
                }

                var list = await _noteRepository.GetAllByNoteTypeAsync(userId, noteTypeId);
                if (list == null || list.Count == 0)
                {
                    _logger.LogInformation("У пользователя({userId}) нет заметок по данному типу({noteTypeId}).", 
                        userId, noteTypeId);
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<List<Note>?> GetFavoriteNotesAsync(int userId)
        {
            _logger.LogInformation("Запрос на получение всех помеченых заметок пользователя({userId}).", userId);
            try
            {
                if (await _userRepository.GetByIdAsync(userId) == null)
                {
                    _logger.LogWarning("Пользователя по Id={userId} не найдено или не существует.", userId);
                    return null;
                }

                var list = await _noteRepository.GetFavoriteNotesAsync(userId);
                if (list == null || list.Count == 0)
                    _logger.LogInformation("У пользователя({userId}) нет помеченых заметок.", userId);

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task CreateAsync(Note note, bool storageMode=false)
        {
            _logger.LogInformation("Создание новой заметки.");

            if (note == null)
            {
                _logger.LogWarning("Объект note был передан пустым.");
                return;
            }

            try
            {
                await _noteRepository.AddAsync(note);
                _logger.LogInformation("Новая заметка ({noteTitle}) успешно создана.", note.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при создании заметки: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            _logger.LogInformation("Обновление заметки {noteTitle}.", note.Title);

            if (note == null)
            {
                _logger.LogWarning("Объект note был передан пустым.");
                return false;
            }

            try
            {
                bool success = await _noteRepository.UpdateAsync(note);

                if (success)
                    _logger.LogInformation("Заметка {noteTitle} обновлёна.", note.Title);
                else
                    _logger.LogWarning("Заметка {noteTitle} не была найдена.", note.Title);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при обновлении заметки: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Удаление заметки по id={noteId}.", id);

            try
            {
                bool success = await _noteRepository.DeleteAsync(id);

                if (success)
                    _logger.LogInformation("Заметка по id={noteId} удалён.", id);
                else
                    _logger.LogWarning("Заметка по id={noteId} не был найден.", id);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при удалении заметки: {errorMessage}", ex);
                throw;
            }
        }
    }
}