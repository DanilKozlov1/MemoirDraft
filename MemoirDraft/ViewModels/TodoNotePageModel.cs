using MemoirDraft.Commands;
using MemoirDraft.Database.DTO;
using MemoirDraft.Database.Models;
using MemoirDraft.DTO;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики страницы TodoNotePage
    /// </summary>
    public class TodoNotePageModel : BaseViewModel
    {
        private readonly ILogger<TodoNotePageModel> _logger;
        private readonly SessionService _sessionService;

        private readonly INoteService _noteService;

        /// <summary>
        /// Название заметки
        /// </summary>
        private string? _title;
        /// <summary>
        /// Текст пункта списка дел
        /// </summary>
        private string? _newTodoText;
        /// <summary>
        /// Список пунктов списка дел
        /// </summary>
        private ObservableCollection<TodoItem> _todoItems;

        /// <summary>
        /// Название заметки
        /// </summary>
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        /// <summary>
        /// Текст пункта списка дел
        /// </summary>
        public string? NewTodoText
        {
            get => _newTodoText;
            set => SetProperty(ref _newTodoText, value);
        }
        /// <summary>
        /// Список пунктов списка дел
        /// </summary>
        public ObservableCollection<TodoItem> TodoItems
        {
            get => _todoItems;
            set => SetProperty(ref _todoItems, value);
        }

        /// <summary>
        /// Команда добавления пункта в список дел
        /// </summary>
        public ICommand AddTodoCommand { get; }
        /// <summary>
        /// Команда удаления пункта списка для todo-заметки
        /// </summary>
        public ICommand RemoveTodoCommand { get; }
        /// <summary>
        /// Команда сохранения заметки
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


        public TodoNotePageModel(ILogger<TodoNotePageModel> logger, SessionService sessionService, 
            INoteService noteService)
        {
            _logger = logger;
            _sessionService = sessionService;

            _noteService = noteService;

            _todoItems = new ObservableCollection<TodoItem>();

            AddTodoCommand = new RelayCommand(AddTodo);
            RemoveTodoCommand = new RelayCommand(RemoveTodo);

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(Save, "Ошибка сохранения"),
                canExecute: () => !IsBusy
            );
            CancelCommand = new RelayCommand(Cancel);
        }


        /// <summary>
        /// Добавление пункта списка дел
        /// </summary>
        private void AddTodo()
        {
            if (!string.IsNullOrWhiteSpace(NewTodoText))
            {
                TodoItems.Add(new TodoItem { Id = TodoItems.Count + 1, Text = NewTodoText, IsDone = false });
                NewTodoText = string.Empty;
            }
        }

        /// <summary>
        /// Удаление пункта для todo-заметки
        /// </summary>
        private void RemoveTodo(object parameter)
        {
            if (parameter is TodoItem item)
                TodoItems.Remove(item);
        }

        /// <summary>
        /// Проверка валидности полей
        /// </summary>
        /// <returns>true - если поля заполнены, false - если пусты</returns>
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
                NoteTypeId = 2,
                Title = Title!,
                TodoItems = TodoItems.ToList(),
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
        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}