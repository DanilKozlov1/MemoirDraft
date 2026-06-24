using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики страницы SimpleNotePage
    /// </summary>
    public class SimpleNotePageModel : BaseViewModel
    {
        private readonly ILogger<SimpleNotePageModel> _logger;

        private readonly SessionService _sessionService;
        private readonly INoteService _noteService;

        /// <summary>
        /// Название заметки
        /// </summary>
        private string? _title;
        /// <summary>
        /// Содержимое
        /// </summary>
        private string? _content;

        /// <summary>
        /// Название заметки
        /// </summary>
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        /// <summary>
        /// Содержимое
        /// </summary>
        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        /// <summary>
        /// Команда сохранения изменений
        /// </summary>
        public ICommand SaveCommand { get; }
        /// <summary>
        /// Команда отмены
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Событие при завершении работы страницы
        /// </summary>
        public event Action<bool?>? CloseRequested;


        public SimpleNotePageModel(ILogger<SimpleNotePageModel> logger, SessionService sessionService, INoteService noteService)
        {
            _logger = logger;
            _sessionService = sessionService;
            _noteService = noteService;

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(Save, "Ошибка создания заметки"),
                canExecute: () => !IsBusy
            );
            CancelCommand = new RelayCommand(Cancel);
        }


        /// <summary>
        /// Проверка валидации данных
        /// </summary>
        /// <returns>true - если поля не пустые, false - если нет</returns>
        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Название не может быть пустым";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Content))
            {
                ErrorMessage = "Содержимое не может быть пустым";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Сохранение заметки
        /// </summary>
        private async Task Save()
        {
            if (!ValidateData())
                return;

            var user = _sessionService.CurrentUser;
            if (user == null)
            {
                ErrorMessage = "Пользователь не авторизован";
                return;
            }

            var note = new Note
            {
                UserId = user.Id,
                NoteTypeId = 1,
                Title = Title!,
                Content = Content,
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _noteService.CreateAsync(note);
            CloseRequested?.Invoke(true);
        }

        /// <summary>
        /// Отмена и закрытие окна
        /// </summary>
        private void Cancel() => CloseRequested?.Invoke(false);
    }
}