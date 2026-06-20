using MemoirDraft.Commands;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class SimpleNotePageModel : BaseViewModel
    {
        private readonly SessionService _sessionService;
        private readonly WindowsService _windowsService;

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


        public SimpleNotePageModel(SessionService sessionService, WindowsService windowsService,
            INoteService noteService)
        {
            _sessionService = sessionService;
            _windowsService = windowsService;

            _noteService = noteService;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }


        private void Save()
        {
            // TODO: передать данные в родительскую ViewModel или сервис
            System.Windows.MessageBox.Show("Сохранение простой заметки");
        }

        private void Cancel()
        {
            // TODO: закрыть окно без сохранения
            System.Windows.MessageBox.Show("Отмена");
        }
    }
}