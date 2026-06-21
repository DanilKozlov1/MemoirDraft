using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class SimpleNotePageModel : BaseViewModel
    {
        private readonly ILogger<SimpleNotePageModel> _logger;
        private readonly SessionService _sessionService;

        private readonly INoteService _noteService;
        private readonly IFileStorageService _fileService;

        private string? _title;
        private string? _content;

        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? CloseRequested;


        public SimpleNotePageModel(ILogger<SimpleNotePageModel> logger, SessionService sessionService, 
            INoteService noteService, IFileStorageService fileService)
        {
            _logger = logger;
            _sessionService = sessionService;

            _noteService = noteService;
            _fileService = fileService;

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(Save, "Ошибка создания заметки"),
                canExecute: () => !IsBusy
            );
            CancelCommand = new RelayCommand(Cancel);
        }


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

            Note note = new Note()
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

            try
            {
                await _fileService.SaveNoteFilesAsync(note);
                _logger.LogInformation("Файлы для заметки {NoteId} сохранены", note.Id);
            }
            catch
            {
                _logger.LogWarning("Ошибка сохранения файлов. Откат БД для заметки {NoteId}", note.Id);
                await _noteService.DeleteAsync(note.Id);

                ErrorMessage = "Ошибка сохранения файлов. Заметка не создана.";
                return;
            }

            CloseRequested?.Invoke(true);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}