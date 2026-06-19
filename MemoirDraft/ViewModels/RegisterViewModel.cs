using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
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

        public ICommand RegistrationCommand { get; }

        public event Action? RegistrationSuccess;


        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;

            RegistrationCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveUserAsync, "Ошибка создания пользователя"),
                canExecute: () => !IsBusy
            );
        }


        private bool ValidateProperty()
        {
            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = "Поле логина не заполнено";
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Поле пароля не заполнено";
                return false;
            }

            return true;
        }

        private async Task SaveUserAsync()
        {
            ErrorMessage = null;

            if (!ValidateProperty())
                return;

            var existing = await _userService.GetByUserNameAsync(Username!);
            if (existing != null)
            {
                ErrorMessage = "Пользователь с таким именем уже существует";
                return;
            }

            var user = new User
            {
                Username = Username!,
                Password = Password!
            };

            try
            {
                await _userService.CreateAsync(user);
                RegistrationSuccess?.Invoke();
                ErrorMessage = null;
            }
            catch
            {
                ErrorMessage = "Ошибка при сохранении пользователя. Проверьте логи.";
            }
        }
    }
}