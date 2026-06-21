using MemoirDraft.Commands;
using MemoirDraft.Database.DTO;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class TodoNotePageModel : BaseViewModel
    {
        private readonly SessionService _sessionService;

        private readonly INoteService _noteService;

        private string? _title;
        private string? _newTodoText;
        private ObservableCollection<TodoItem> _todoItems;

        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string? NewTodoText
        {
            get => _newTodoText;
            set => SetProperty(ref _newTodoText, value);
        }

        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set => SetProperty(ref _todoItems, value);
        }

        public ICommand AddTodoCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? CloseRequested;


        public TodoNotePageModel(SessionService sessionService, INoteService noteService)
        {
            _sessionService = sessionService;

            _noteService = noteService;

            _todoItems = new ObservableCollection<TodoItem>();

            AddTodoCommand = new RelayCommand(AddTodo);
            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(Save, "Ошибка сохранения"),
                canExecute: () => !IsBusy
            );
            CancelCommand = new RelayCommand(Cancel);
        }


        private void AddTodo()
        {
            if (!string.IsNullOrWhiteSpace(NewTodoText))
            {
                TodoItems.Add(new TodoItem { Id = TodoItems.Count + 1, Text = NewTodoText, IsDone = false });
                NewTodoText = string.Empty;
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Название не может быть пустым";
                return false;
            }
            if (TodoItems == null || TodoItems.Count == 0)
            {
                ErrorMessage = "Добавьте хотя бы одну задачу";
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

            var list = TodoItems.ToList();

            Note note = new Note()
            {
                UserId = user.Id,
                NoteTypeId = 2,
                Title = Title!,
                TodoItems = list,
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