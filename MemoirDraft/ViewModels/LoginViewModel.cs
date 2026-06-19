using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private SessionService _sessionService;
        private readonly IUserService _userService;

        private string? _username;
        private string? _password;

        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        public event Action? CloseRequested;


        public LoginViewModel(SessionService sessionService, IUserService userService)
        {
            _sessionService = sessionService;
            _userService = userService;

            LoginCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(CheckAndAuthAsync, "Ошибка аунтефикации"),
                canExecute: () => !IsBusy
            );
        }


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

            // Временное
            MessageBox.Show("ENTER");
            CloseRequested?.Invoke();
        }
    }
}