using MemoirDraft.Commands;
using MemoirDraft.DTO;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.Utils;
using MemoirDraft.ViewModels.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class MainWindowModel : BaseViewModel
    {
        private readonly ILogger<MainWindowModel> _logger;

        private readonly SessionService _sessionService;
        private readonly WindowsService _windowsService;
        private readonly IFileStorageService _fileService;

        private readonly INoteService _noteService;
        private readonly INoteTypeService _noteTypeService;
        // Удалить после тестов
        private readonly IUserService _userService;

        private ObservableCollection<NoteDto> _notes;

        private ObservableCollection<FilterDto> _filters;
        private int _currentFilterId;

        public ObservableCollection<NoteDto> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public ObservableCollection<FilterDto> Filters
        {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        public ICommand CreateNoteCommand { get; }
        public ICommand OpenNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand CloseCommand { get; }


        private async Task LoadUser()
        {
            try
            {
                var user = await _userService.GetByIdAsync(1);
                _sessionService.CurrentUser = user;
                System.Diagnostics.Debug.WriteLine($"Загружен пользователь: {user?.Username ?? "null"}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в LoadUser: {ex.Message}");
                _logger.LogWarning("Error: {ex}", ex);
            }
        }


        public MainWindowModel(ILogger<MainWindowModel> logger,
            SessionService sessionService, WindowsService windowsService, IFileStorageService fileService,
            INoteService noteService, INoteTypeService noteTypeService, IUserService userService)
        {
            _logger = logger;

            _sessionService = sessionService;
            _fileService = fileService;
            _windowsService = windowsService;

            _userService = userService; // Удалить после тестов

            _noteService = noteService;
            _noteTypeService = noteTypeService;

            _notes = new ObservableCollection<NoteDto>();

            _filters = new ObservableCollection<FilterDto>();
            _currentFilterId = 0;

            CreateNoteCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(CreateNoteAsync, ""),
                canExecute: () => !IsBusy
            );
            OpenNoteCommand = new RelayCommandAsync(
                execute: OpenNoteAsync,
                canExecute: _ => !IsBusy
            );
            DeleteNoteCommand = new RelayCommandAsync(
                execute: DeleteNoteAsync,
                canExecute: _ => !IsBusy
            );

            FilterCommand = new RelayCommandAsync(
                execute: FilterAsync,
                canExecute: _ => !IsBusy
            );
            LoadDataCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadDataAsync, "Ошибка загрузки данных"),
                canExecute: () => !IsBusy
            );

            CloseCommand = new RelayCommand(Close);

            LoadDataCommand.Execute(null);
        }


        private async Task LoadFiltersAsync()
        {
            var types = await _noteTypeService.GetAllAsync();
            Filters.Clear();

            Filters.Add(new FilterDto { Id = 0, Name = "Все", IsActive = true, IsSpecial = true });
            Filters.Add(new FilterDto { Id = -1, Name = "⭐ Избранные", IsActive = false, IsSpecial = true });

            if (types != null)
            {
                foreach (var type in types)
                {
                    Filters.Add(new FilterDto
                    {
                        Id = type.Id,
                        Name = type.Name,
                        IsActive = false,
                        IsSpecial = false
                    });
                }
            }
        }

        private async Task LoadNotesAsync()
        {
            Notes.Clear();

            var userId = _sessionService.CurrentUser?.Id ?? 0;
            List<NoteDto> noteDtos;

            if (_currentFilterId == 0)
            {
                var notes = await _noteService.GetAllByUserAsync(userId);
                noteDtos = notes?.ToDtos() ?? new List<NoteDto>();
            }
            else if (_currentFilterId == -1)
            {
                var notes = await _noteService.GetFavoriteNotesAsync(userId);
                noteDtos = notes?.ToDtos() ?? new List<NoteDto>();
            }
            else
            {
                var notes = await _noteService.GetAllByNoteTypeAsync(userId, _currentFilterId);
                noteDtos = notes?.ToDtos() ?? new List<NoteDto>();
            }

            Notes = new ObservableCollection<NoteDto>(noteDtos);
        }

        private async Task LoadDataAsync()
        {
            await LoadUser();
            if (_sessionService.CurrentUser == null)
            {
                ErrorMessage = "Не удалось загрузить пользователя. Проверьте подключение к БД.";
                return;
            }
            await LoadFiltersAsync();
            await LoadNotesAsync();
        }

        private async Task FilterAsync(object parameter)
        {
            if (parameter is FilterDto filter)
            {
                foreach (var f in Filters)
                    f.IsActive = false;
                filter.IsActive = true;

                _currentFilterId = filter.Id;
                await LoadNotesAsync();
            }
        }

        private async Task CreateNoteAsync()
        {
            var result = _windowsService.OpenCreateNote();

            if (result == true) 
                await LoadNotesAsync();
        }

        private async Task OpenNoteAsync(object parameter)
        {
            if (parameter is NoteDto noteDto)
            {
                var result = _windowsService.OpenNoteView(noteDto.Id);
                if (result == true)
                    await LoadNotesAsync();
            }
        }

        private async Task DeleteNoteAsync(object parameter)
        {
            if (parameter is not NoteDto noteDto)
                return;

            try
            {
                var success = await _noteService.DeleteAsync(noteDto.Id);
                if (!success)
                {
                    ErrorMessage = "Заметка не найдена в базе данных";
                    return;
                }

                try
                {
                    await _fileService.DeleteNoteFilesAsync(noteDto.Id);
                    _logger.LogInformation("Файлы для заметки {NoteId} удалены", noteDto.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ошибка при удалении файлов заметки {NoteId}", noteDto.Id);
                }

                Notes.Remove(noteDto);
                _logger.LogInformation("Заметка {NoteId} удалена", noteDto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении заметки {NoteId}", noteDto.Id);
                ErrorMessage = "Ошибка при удалении заметки";
            }
        }

        private void Close()
        {
            _sessionService.CurrentUser = null;
            //_windowsService.OpenAuthorization();
            _windowsService.CloseWindow(this);
        }
    }
}