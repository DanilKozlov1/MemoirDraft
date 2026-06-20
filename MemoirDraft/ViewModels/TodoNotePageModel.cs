using MemoirDraft.Commands;
using MemoirDraft.Database.DTO;
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
        private readonly WindowsService _windowsService;

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


        public TodoNotePageModel(SessionService sessionService, WindowsService windowsService,
            INoteService noteService)
        {
            _sessionService = sessionService;
            _windowsService = windowsService;

            _noteService = noteService;

            _todoItems = new ObservableCollection<TodoItem>();

            AddTodoCommand = new RelayCommand(AddTodo);
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }


        private void AddTodo()
        {
            if (!string.IsNullOrWhiteSpace(NewTodoText))
            {
                TodoItems.Add(new TodoItem { Id = TodoItems.Count + 1, Text = NewTodoText, IsDone = false });
                NewTodoText = string.Empty;
            }
        }

        private void Save()
        {
            // TODO: передать данные в родительскую ViewModel или сервис
            System.Windows.MessageBox.Show("Сохранение TODO-заметки");
        }

        private void Cancel()
        {
            // TODO: закрыть окно без сохранения
            System.Windows.MessageBox.Show("Отмена");
        }
    }
}