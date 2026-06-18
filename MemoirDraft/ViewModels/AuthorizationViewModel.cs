using MemoirDraft.Commands;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {
        private object? _currentPage;
        private readonly LoginPageModel _loginPage;
        private readonly RegisterPageModel _registerPage;

        public object? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public ICommand SwitchToLoginCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand CloseCommand { get; }


        public AuthorizationViewModel(LoginPageModel loginPage, RegisterPageModel registerPage)
        {
            _loginPage = loginPage;
            _registerPage = registerPage;

            CurrentPage = _loginPage;

            SwitchToLoginCommand = new RelayCommand(() => CurrentPage = _loginPage);
            SwitchToRegisterCommand = new RelayCommand(() => CurrentPage = _registerPage);
        }
    }
}