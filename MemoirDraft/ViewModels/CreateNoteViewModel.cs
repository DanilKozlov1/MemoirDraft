using MemoirDraft.Commands;
using MemoirDraft.DTO;
using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class CreateNoteViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        private readonly SimpleNotePageModel _simplePage;
        private readonly TodoNotePageModel _todoPage;

        private ObservableCollection<CreateNotePageDto> _pages;
        private CreateNotePageDto? _activePage;

        public ObservableCollection<CreateNotePageDto> Pages
        {
            get => _pages;
            set => SetProperty(ref _pages, value);
        }

        public CreateNotePageDto? ActivePage
        {
            get => _activePage;
            set => SetProperty(ref _activePage, value);
        }

        public ICommand SwitchPageCommand { get; }
        public ICommand CloseCommand { get; }


        public CreateNoteViewModel(WindowsService windowsService, 
            SimpleNotePageModel simpleNoteVm, TodoNotePageModel todoNoteVm)
        {
            _windowsService = windowsService;
            _simplePage = simpleNoteVm;
            _todoPage = todoNoteVm;

            _pages = new ObservableCollection<CreateNotePageDto>();
            Pages.Add(new CreateNotePageDto { Name = "📄 Обычная", Content = _simplePage, IsActive = true });
            Pages.Add(new CreateNotePageDto { Name = "📋 TODO", Content = _todoPage, IsActive = false });
            ActivePage = Pages.First();

            SwitchPageCommand = new RelayCommand(SwitchPage);
            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this, false));
        }


        private void SetActivePage(CreateNotePageDto page)
        {
            if (ActivePage == page) 
                return;
            
            foreach (var t in Pages) 
                t.IsActive = false;
            
            page.IsActive = true;
            ActivePage = page;
        }

        private void SwitchPage(object parameter)
        {
            if (parameter is CreateNotePageDto tab)
                SetActivePage(tab);
        }
    }
}