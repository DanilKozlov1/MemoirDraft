using MemoirDraft.Commands;
using MemoirDraft.DTO;
using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики окна CreateNoteView
    /// </summary>
    public class CreateNoteViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        private readonly SimpleNotePageModel _simplePage;
        private readonly TodoNotePageModel _todoPage;

        /// <summary>
        /// Список dto для отображения окна
        /// </summary>
        private ObservableCollection<CreateNotePageDto> _pages;
        /// <summary>
        /// Активная страница
        /// </summary>
        private CreateNotePageDto? _activePage;

        /// <summary>
        /// Список dto для отображения окна
        /// </summary>
        public ObservableCollection<CreateNotePageDto> Pages
        {
            get => _pages;
            set => SetProperty(ref _pages, value);
        }
        /// <summary>
        /// Активная страница
        /// </summary>
        public CreateNotePageDto? ActivePage
        {
            get => _activePage;
            set => SetProperty(ref _activePage, value);
        }

        /// <summary>
        /// Команда смены страницы
        /// </summary>
        public ICommand SwitchPageCommand { get; }
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public CreateNoteViewModel(WindowsService windowsService, 
            SimpleNotePageModel simpleNoteVm, TodoNotePageModel todoNoteVm)
        {
            _windowsService = windowsService;

            _simplePage = simpleNoteVm;
            _todoPage = todoNoteVm;

            _simplePage.CloseRequested += OnPageCloseRequested;
            _todoPage.CloseRequested += OnPageCloseRequested;

            _pages = new ObservableCollection<CreateNotePageDto>();
            Pages.Add(new CreateNotePageDto { Name = "📄 Обычная", Content = _simplePage, IsActive = true });
            Pages.Add(new CreateNotePageDto { Name = "📋 TODO", Content = _todoPage, IsActive = false });
            ActivePage = Pages.First();

            SwitchPageCommand = new RelayCommand(SwitchPage);
            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this, false));
        }


        /// <summary>
        /// Смена активной страницы
        /// </summary>
        /// <param name="page">Dto страницы</param>
        private void SetActivePage(CreateNotePageDto page)
        {
            if (ActivePage == page) 
                return;
            
            foreach (var t in Pages) 
                t.IsActive = false;
            
            page.IsActive = true;
            ActivePage = page;
        }

        /// <summary>
        /// Установка страницы в окно
        /// </summary>
        /// <param name="parameter">Dto страницы</param>
        private void SwitchPage(object parameter)
        {
            if (parameter is CreateNotePageDto tab)
                SetActivePage(tab);
        }

        /// <summary>
        /// Закрытие окна при окончании работы страницы
        /// </summary>
        /// <param name="result">Результат работы страницы</param>
        private void OnPageCloseRequested(bool? result)
        {
            _windowsService.CloseWindow(this, result);
        }
    }
}