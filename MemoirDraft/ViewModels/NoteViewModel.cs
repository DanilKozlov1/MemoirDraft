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
        /// Команда загрузки данных заметки
        /// </summary>
        public ICommand LoadCommand { get; }
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

        /// <summary>
        /// Сохранение изменений заметки
        /// </summary>
        private async Task SaveAsync()
        {
            Note? note = await _noteService.GetByIdAsync(_noteId);
            if (note == null)
            {
                ErrorMessage = "Ошибка доступа к заметке";
                return;
            }

            note.Title = Title;
            
            if (!IsTodo)
                note.Content = Content;
            else
            {
                var todoItems = new List<TodoItem>();
                foreach (var item in TodoItems)
                {
                    var todoItem = new TodoItem()
                    {
                        Id = item.Id,
                        IsDone = item.IsDone,
                        Text = item.Text
                    };

                    todoItems.Add(todoItem);
                }

                note.TodoItems = todoItems;
            }

            await _noteService.UpdateAsync(note);
            _windowsService.CloseWindow(this, true);
        }
    }
}