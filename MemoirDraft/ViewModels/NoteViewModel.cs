using MemoirDraft.Commands;
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
    public class NoteViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;
        private readonly ILogger<NoteViewModel> _logger;
        private readonly INoteService _noteService;

        private string _title = string.Empty;
        private string _content = string.Empty;
        private ObservableCollection<TodoItemDto> _todoItems = new();
        private int _noteTypeId;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public ObservableCollection<TodoItemDto> TodoItems
        {
            get => _todoItems;
            set => SetProperty(ref _todoItems, value);
        }

        public bool IsTodo => NoteTypeId == 2;

        public int NoteTypeId
        {
            get => _noteTypeId;
            set
            {
                SetProperty(ref _noteTypeId, value);
                OnPropertyChanged(nameof(IsTodo));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }

        public event Action<bool?>? CloseRequested;

        public NoteViewModel(
            ILogger<NoteViewModel> logger,
            INoteService noteService,
            WindowsService windowsService,
            int noteId)
        {
            _logger = logger;
            _noteService = noteService;
            _windowsService = windowsService;

            LoadCommand = new RelayCommandAsync(
                execute: async (parameter) => await LoadNoteAsync((int)parameter),
                canExecute: _ => !IsBusy
            );

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveAsync, "Ошибка сохранения"),
                canExecute: () => !IsBusy
            );

            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this, false));

            LoadCommand.Execute(noteId);
        }

        private async Task LoadNoteAsync(int noteId)
        {
            var note = await _noteService.GetByIdAsync(noteId);
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
                        Text = item.Text,
                        IsDone = item.IsDone
                    });
                }
            }
        }

        private async Task SaveAsync()
        {
            // Сохранение (заглушка)
            await Task.Delay(100);
            CloseRequested?.Invoke(true);
        }
    }
}