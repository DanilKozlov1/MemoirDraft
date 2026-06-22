using MemoirDraft.Commands;
using MemoirDraft.Services;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики окна AuthorizationView
    /// </summary>
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        /// <summary>
        /// Текущая страница
        /// </summary>
        private object? _currentPage;
        /// <summary>
        /// Страница входа
        /// </summary>
        private readonly LoginPageModel _loginPage;
        /// <summary>
        /// Страница регистрации
        /// </summary>
        private readonly RegisterPageModel _registerPage;

        /// <summary>
        /// Текущая страница
        /// </summary>
        public object? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        /// <summary>
        /// Комадна меняющая текущую страницу на _loginPage
        /// </summary>
        public ICommand SwitchToLoginCommand { get; }
        /// <summary>
        /// Команда меняющая текущую страницу на _registerPage
        /// </summary>
        public ICommand SwitchToRegisterCommand { get; }
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public AuthorizationViewModel(WindowsService windowsService, 
            LoginPageModel loginPage, RegisterPageModel registerPage)
        {
            _windowsService = windowsService;

            _loginPage = loginPage;
            _loginPage.CloseRequested += () =>
            {
                _windowsService.OpenMainWindow();
                _windowsService.CloseWindow(this);
            };

            _registerPage = registerPage;
            _registerPage.RegistrationSuccess += () => CurrentPage = _loginPage;

            CurrentPage = _loginPage;

            SwitchToLoginCommand = new RelayCommand(() => CurrentPage = _loginPage);
            SwitchToRegisterCommand = new RelayCommand(() => CurrentPage = _registerPage);
            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this));
        }
    }
}