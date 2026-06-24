using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики страницы LoginPage
    /// </summary>
    public class LoginPageModel : BaseViewModel
    {
        private SessionService _sessionService;
        private readonly IUserService _userService;

        /// <summary>
        /// Логин
        /// </summary>
        private string? _username;
        /// <summary>
        /// Пароль
        /// </summary>
        private string? _password;

        /// <summary>
        /// Логин
        /// </summary>
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        /// <summary>
        /// Пароль
        /// </summary>
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Команда входа
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Событие, необходимое для закрытия родительского окна
        /// </summary>
        public event Action? CloseRequested;


        public LoginPageModel(SessionService sessionService, IUserService userService)
        {
            _sessionService = sessionService;
            _userService = userService;

            LoginCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(CheckAndAuthAsync, "Ошибка аунтефикации"),
                canExecute: () => !IsBusy
            );
        }


        /// <summary>
        /// Проверка данных и вход
        /// </summary>
        private async Task CheckAndAuthAsync()
        {
            ErrorMessage = null;

            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = "Поле логина не заполнено";
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Поле пароля не заполнено";
                return;
            }

            User? user = await _userService.AuthAsync(Username, Password);
            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                return;
            }

            _sessionService.CurrentUser = user;
            CloseRequested?.Invoke();
        }
    }
}