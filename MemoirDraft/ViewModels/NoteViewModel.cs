using MemoirDraft.Commands;
using MemoirDraft.Database.DTO;
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
    /// Модель логики окна NoteView
    /// </summary>
    public class NoteViewModel : BaseViewModel
    {
        private readonly ILogger<NoteViewModel> _logger;
        
        private readonly WindowsService _windowsService;
        private readonly INoteService _noteService;

        /// <summary>
        /// Id текущей открытой заметки
        /// </summary>
        private readonly int _noteId;

        /// <summary>
        /// Id типа текущей заметки
        /// </summary>
        private int _noteTypeId;
        /// <summary>
        /// Название текущей заметки
        /// </summary>
        private string _title = string.Empty;
        /// <summary>
        /// Содержимое заметки для типа simple
        /// </summary>
        private string _content = string.Empty;
        /// <summary>
        /// Список пунктов для заметки типа todo
        /// </summary>
        private ObservableCollection<TodoItemDto> _todoItems;
        /// <summary>
        /// Новый пункт для заметки типа todo
        /// </summary>
        private string _newTodoText = string.Empty;

        /// <summary>
        /// Id типа текущей заметки
        /// </summary>
        public int NoteTypeId
        {
            get => _noteTypeId;
            set
            {
                SetProperty(ref _noteTypeId, value);
                OnPropertyChanged(nameof(IsTodo));
            }
        }
        /// <summary>
        /// Является ли заметка списком дел
        /// </summary>
        public bool IsTodo => NoteTypeId == 2;
        /// <summary>
        /// Название текущей заметки
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        /// <summary>
        /// Содержимое заметки для типа simple
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
        /// <summary>
        /// Список пунктов для заметки типа todo
        /// </summary>
        public ObservableCollection<TodoItemDto> TodoItems
        {
            get => _todoItems;
            set => SetProperty(ref _todoItems, value);
        }
        /// <summary>
        /// Новый пункт для заметки типа todo
        /// </summary>
        public string NewTodoText
        {
            get => _newTodoText;
            set => SetProperty(ref _newTodoText, value);
        }

        /// <summary>
        /// Команда загрузки данных заметки
        /// </summary>
        public ICommand LoadCommand { get; }
        
        /// <summary>
        /// Команда добавления нового пункта списка для todo-заметки
        /// </summary>
        public ICommand AddTodoCommand { get; }
        /// <summary>
        /// Команда перемещения пункта списка todo-заметки вверх
        /// </summary>
        public ICommand MoveTodoUpCommand { get; }
        /// <summary>
        /// Команда перемещения пункта списка todo-заметки вниз
        /// </summary>
        public ICommand MoveTodoDownCommand { get; }
        /// <summary>
        /// Команда удаления пункта списка для todo-заметки
        /// </summary>
        public ICommand RemoveTodoCommand { get; }
        /// <summary>
        /// Команда сохранения изменений
        /// </summary>
        public ICommand SaveCommand { get; }
        
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public NoteViewModel(ILogger<NoteViewModel> logger, WindowsService windowsService,
            INoteService noteService, int noteId)
        {
            _logger = logger;
            _windowsService = windowsService;

            _noteService = noteService;

            _noteId = noteId;
            _todoItems = new ObservableCollection<TodoItemDto>();

            LoadCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadNoteAsync, "Ошибка загрузки заметки"),
                canExecute: () => !IsBusy
            );

            AddTodoCommand = new RelayCommand(
                execute: AddTodo,
                canExecute: () => !string.IsNullOrWhiteSpace(NewTodoText)
            );
            MoveTodoUpCommand = new RelayCommand(
                execute:  MoveTodoUp,
                canExecute: CanMoveUp
            );
            MoveTodoDownCommand = new RelayCommand(
                execute: MoveTodoDown,
                canExecute: CanMoveDown
            );
            RemoveTodoCommand = new RelayCommand(RemoveTodo);

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveAsync, "Ошибка сохранения"),
                canExecute: () => !IsBusy
            );

            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this, false));

            LoadCommand.Execute(null);
        }


        /// <summary>
        /// Загрузка данных заметки
        /// </summary>
        /// <param name="noteId">Id передаваемой заметки</param>
        private async Task LoadNoteAsync()
        {
            var note = await _noteService.GetByIdAsync(_noteId);
            if (note == null)
            {
                ErrorMessage = "Заметка не найдена";
                return;
            }

            Title = note.Title;
            Content = note.Content ?? string.Empty;
            NoteTypeId = note.NoteTypeId;

            if (note.TodoItems != null)
            {
                TodoItems.Clear();
                foreach (var item in note.TodoItems)
                {
                    TodoItems.Add(new TodoItemDto
                    {
                        Id = item.Id,
                        Text = item.Text!,
                        IsDone = item.IsDone
                    });
                }
            }
        }

        #region -- Методы для Todo-заметки --
        /// <summary>
        /// Добавление нового пункта для todo-заметки
        /// </summary>
        private void AddTodo()
        {
            if (string.IsNullOrWhiteSpace(NewTodoText)) 
                return;

            TodoItems.Add(new TodoItemDto
            {
                Id = TodoItems.Count > 0 ? TodoItems.Max(t => t.Id) + 1 : 1,
                Text = NewTodoText,
                IsDone = false
            });
            
            NewTodoText = string.Empty;
        }

        /// <summary>
        /// Проверка возможности переместить пункт списка todo вверх
        /// </summary>
        /// <param name="parameter">TodoItemDto</param>
        /// <returns>true - если перемещение возможно, false - если нет</returns>
        private bool CanMoveUp(object parameter)
        {
            if (parameter is TodoItemDto item)
            {
                int index = TodoItems.IndexOf(item);
                return index > 0;
            }
            return false;
        }

        /// <summary>
        /// Проверка возможности переместить пункт списка todo вниз
        /// </summary>
        /// <param name="parameter">TodoItemDto</param>
        /// <returns>true - если перемещение возможно, false - если нет</returns>
        private bool CanMoveDown(object parameter)
        {
            if (parameter is TodoItemDto item)
            {
                int index = TodoItems.IndexOf(item);
                return index >= 0 && index < TodoItems.Count - 1;
            }
            return false;
        }

        /// <summary>
        /// Перемещение пункта списка todo вверх
        /// </summary>
        /// <param name="parameter">TodoItemDto</param>
        private void MoveTodoUp(object parameter)
        {
            if (parameter is TodoItemDto item)
            {
                int index = TodoItems.IndexOf(item);
                if (index > 0)
                {
                    TodoItems.Move(index, index - 1);

                    (MoveTodoUpCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (MoveTodoDownCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Перемещение пункта списка todo вниз
        /// </summary>
        /// <param name="parameter">TodoItemDto</param>
        private void MoveTodoDown(object parameter)
        {
            if (parameter is TodoItemDto item)
            {
                int index = TodoItems.IndexOf(item);
                if (index >= 0 && index < TodoItems.Count - 1)
                {
                    TodoItems.Move(index, index + 1);

                    (MoveTodoUpCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (MoveTodoDownCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Удаление пункта для todo-заметки
        /// </summary>
        private void RemoveTodo(object parameter)
        {
            if (parameter is TodoItemDto item)
                TodoItems.Remove(item);
        }
        #endregion

        /// <summary>
        /// Сохранение изменений заметки
        /// </summary>
        private async Task SaveAsync()
        {
            var note = await _noteService.GetByIdAsync(_noteId);
            if (note == null)
            {
                ErrorMessage = "Ошибка доступа к заметке";
                return;
            }

            note.Title = Title;
            note.UpdatedAt = DateTime.UtcNow;

            if (!IsTodo)
            {
                note.Content = Content;
                note.TodoItems = null;
            }
            else
            {
                note.Content = null;
                note.TodoItems = TodoItems.Select(item => new TodoItem
                {
                    Id = item.Id,
                    Text = item.Text,
                    IsDone = item.IsDone
                }).ToList();
            }

            await _noteService.UpdateAsync(note);
            _logger.LogInformation("Заметка {NoteId} обновлена", _noteId);

            _windowsService.CloseWindow(this, true);
        }
    }
}