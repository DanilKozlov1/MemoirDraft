using MemoirDraft.Commands;
using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        private object? _currentPage;
        private readonly LoginViewModel _loginPage;
        private readonly RegisterViewModel _registerPage;

        public object? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public ICommand SwitchToLoginCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand CloseCommand { get; }


        public AuthorizationViewModel(WindowsService windowsService, 
            LoginViewModel loginPage, RegisterViewModel registerPage)
        {
            _windowsService = windowsService;
            _loginPage = loginPage;
            _registerPage = registerPage;

            CurrentPage = _loginPage;

            SwitchToLoginCommand = new RelayCommand(() => CurrentPage = _loginPage);
            SwitchToRegisterCommand = new RelayCommand(() => CurrentPage = _registerPage);
            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this));
        }
    }
}