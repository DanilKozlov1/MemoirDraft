using MemoirDraft.Commands;
using MemoirDraft.DTO;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.Utils;
using MemoirDraft.ViewModels.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики окна MainWindow
    /// </summary>
    public class MainWindowModel : BaseViewModel
    {
        private readonly ILogger<MainWindowModel> _logger;
        private readonly IConfiguration _config;

        private readonly SessionService _sessionService;
        private readonly WindowsService _windowsService;

        private readonly INoteService _noteService;
        private readonly INoteTypeService _noteTypeService;

        /// <summary>
        /// Список dto заметок для отображения
        /// </summary>
        private ObservableCollection<NoteDto> _notes;
        /// <summary>
        /// Список dto типов заметок для отображения и фильтра
        /// </summary>
        private ObservableCollection<FilterDto> _filters;
        /// <summary>
        /// Id выбранного фильтра (типа заметок)
        /// </summary>
        private int _currentFilterId;

        /// <summary>
        /// Список dto заметок для отображения
        /// </summary>
        public ObservableCollection<NoteDto> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
        /// <summary>
        /// Список dto типов заметок для отображения и фильтра
        /// </summary>
        public ObservableCollection<FilterDto> Filters
        {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        /// <summary>
        /// Команда создания новой заметки
        /// </summary>
        public ICommand CreateNoteCommand { get; }
        /// <summary>
        /// Команда открытия заметки
        /// </summary>
        public ICommand OpenNoteCommand { get; }
        /// <summary>
        /// Команда удаления заметки
        /// </summary>
        public ICommand DeleteNoteCommand { get; }
        /// <summary>
        /// Команда добавления заметки в "Избранные"
        /// </summary>
        public ICommand ToggleFavoriteCommand { get; }
        /// <summary>
        /// Команда фильтрации списка заметок
        /// </summary>
        public ICommand FilterCommand { get; }
        /// <summary>
        /// Команда загрузки данных
        /// </summary>
        public ICommand LoadDataCommand { get; }
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public MainWindowModel(ILogger<MainWindowModel> logger, IConfiguration config,
            SessionService sessionService, WindowsService windowsService,
            INoteService noteService, INoteTypeService noteTypeService)
        {
            _logger = logger;
            _config = config;

            _sessionService = sessionService;
            _windowsService = windowsService;

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
            ToggleFavoriteCommand = new RelayCommandAsync(
                execute: ToggleFavoriteAsync,
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


        /// <summary>
        /// Загрузка списка фильтров
        /// </summary>
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

        /// <summary>
        /// Загрузка списка заметок
        /// </summary>
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

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private async Task LoadDataAsync()
        {
            if (_sessionService.NoAuth)
                await _sessionService.LoadUser();

            if (_sessionService.CurrentUser == null)
            {
                ErrorMessage = "Не удалось загрузить пользователя. Проверьте подключение к БД.";
                return;
            }

            await LoadFiltersAsync();
            await LoadNotesAsync();
        }

        /// <summary>
        /// Применение фильтра
        /// </summary>
        /// <param name="parameter">FilterDto</param>
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

        /// <summary>
        /// Открытие окна создания новой заметки (CreateNoteView)
        /// </summary>
        private async Task CreateNoteAsync()
        {
            var result = _windowsService.OpenCreateNote();

            if (result == true) 
                await LoadNotesAsync();
        }

        /// <summary>
        /// Открытие окна содержимого заметки (NoteView)
        /// </summary>
        /// <param name="parameter">NoteDto</param>
        private async Task OpenNoteAsync(object parameter)
        {
            if (parameter is NoteDto noteDto)
            {
                var result = _windowsService.OpenNoteView(noteDto.Id);
                if (result == true)
                    await LoadNotesAsync();
            }
        }

        /// <summary>
        /// Удаление заметки из списка
        /// </summary>
        /// <param name="parameter">NoteDto</param>
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

                Notes.Remove(noteDto);
                _logger.LogInformation("Заметка {NoteId} удалена", noteDto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении заметки {NoteId}", noteDto.Id);
                ErrorMessage = "Ошибка при удалении заметки";
            }
        }

        /// <summary>
        /// Добавление заметки в избранное
        /// </summary>
        /// <param name="parameter">NoteDto</param>
        private async Task ToggleFavoriteAsync(object parameter)
        {
            if (parameter is NoteDto noteDto)
            {
                var note = await _noteService.GetByIdAsync(noteDto.Id);
                if (note == null) return;

                note.IsFavorite = !note.IsFavorite;
                await _noteService.UpdateAsync(note);

                if (note.IsFavorite)
                    _logger.LogInformation("Заметка по id={noteId} добавлена в избранное.", note.Id);
                else
                    _logger.LogInformation("Заметка по id={noteId} убрана из избранного.", note.Id);

                var target = Notes.FirstOrDefault(n => n.Id == noteDto.Id);
                if (target != null)
                    target.IsFavorite = note.IsFavorite;
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        private void Close()
        {
            _sessionService.CurrentUser = null;

            if (_sessionService.NoAuth)
            {
                _logger.LogInformation("Запрос на закрытие окна для ViewModel: {ViewModelName}", GetType().Name);
                Application.Current.Shutdown();
            }
            else
            {
                _windowsService.OpenAuthorization();
                _windowsService.CloseWindow(this);
            }
        }
    }
}