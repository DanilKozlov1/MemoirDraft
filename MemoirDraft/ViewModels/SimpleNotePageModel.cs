using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class SimpleNotePageModel : BaseViewModel
    {
        private readonly SessionService _sessionService;

        private readonly INoteService _noteService;

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


        public SimpleNotePageModel(SessionService sessionService, INoteService noteService)
        {
            _sessionService = sessionService;

            _noteService = noteService;

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
            CloseRequested?.Invoke(true);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}