using MemoirDraft.Commands;
using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using MemoirDraft.ViewModels.Abstractions;
using System.Windows.Input;

namespace MemoirDraft.ViewModels
{
    /// <summary>
    /// Модель логики страницы RegisterPage
    /// </summary>
    public class RegisterPageModel : BaseViewModel
    {
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
        /// Команда регистрации пользователя
        /// </summary>
        public ICommand RegistrationCommand { get; }

        /// <summary>
        /// Событие прошла ли регистрация
        /// </summary>
        public event Action? RegistrationSuccess;


        public RegisterPageModel(IUserService userService)
        {
            _userService = userService;

            RegistrationCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveUserAsync, "Ошибка создания пользователя"),
                canExecute: () => !IsBusy
            );
        }


        /// <summary>
        /// Проверка валидности полей
        /// </summary>
        /// <returns>true - если поля заполнены, false - если пусты</returns>
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

        /// <summary>
        /// Сохранение нового пользователя
        /// </summary>
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