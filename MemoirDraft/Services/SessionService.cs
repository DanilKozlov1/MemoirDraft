using MemoirDraft.Database.Models;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services
{
    /// <summary>
    /// Класc-сервис для хранения текущего пользователя
    /// </summary>
    public class SessionService
    {
        private readonly ILogger<SessionService> _logger;
        private readonly IConfiguration _config;

        private readonly IUserService _userService;

        /// <summary>
        /// Режим входа в приложение
        /// </summary>
        private bool _noAuth;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private User? _currentUser;

        /// <summary>
        /// Режим входа в приложение
        /// </summary>
        public bool NoAuth
        {
            get => _noAuth;
            set => _noAuth = value;
        }
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser?.Id != value?.Id)
                {
                    _currentUser = value;
                    CurrentUserChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Ивент при изменении текущего пользователя
        /// </summary>
        public event Action? CurrentUserChanged;


        public SessionService(ILogger<SessionService> logger, IConfiguration config,
            IUserService userService)
        {
            _logger = logger;
            _config = config;

            _userService = userService;

            var appMode = _config.GetValue<string>("Settings:AppMode") ?? "Auth";
            if (appMode == "NoAuth")
                NoAuth = true;
            else
                NoAuth = false;
        }


        public async Task LoadUser()
        {
            if (NoAuth)
            {
                try
                {
                    var user = await _userService.GetByIdAsync(1);
                    if (user != null)
                    {
                        CurrentUser = user;
                        _logger.LogInformation("Загружен пользователь {Username} из БД (режим NoAuth)", user.Username);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, 
                        "Не удалось загрузить пользователя из БД (режим NoAuth). Будет создан виртуальный пользователь.");
                }

                CurrentUser = new User
                {
                    Id = 1,
                    Username = "Nothing",
                    Password = "123"
                };
                _logger.LogInformation("Создан виртуальный пользователь для режима NoAuth (БД недоступна)");
            }
            else
            {
                try
                {
                    var user = await _userService.GetByIdAsync(1);
                    if (user == null)
                    {
                        _logger.LogError("Критическая ошибка: Пользователь с ID=1 не найден в БД!");
                        throw new InvalidOperationException("Пользователь с ID=1 не найден");
                    }

                    CurrentUser = user;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка подключения к базе данных при загрузке пользователя.");
                    throw;
                }
            }
        }
    }
}