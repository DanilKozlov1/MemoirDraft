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
        private readonly IServiceProvider _services;
        private readonly ILogger<SessionService> _logger;

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


        public SessionService(IServiceProvider services, ILogger<SessionService> logger)
        {
            _services = services;
            _logger = logger;

            var config = _services.GetRequiredService<IConfiguration>();
            var appMode = config.GetValue<string>("Settings:AppMode") ?? "Auth";
            if (appMode == "NoAuth")
                NoAuth = true;
            else
                NoAuth = false;
        }


        public async Task LoadUser()
        {
            IUserService userService = _services.GetRequiredService<IUserService>();

            try
            {
                var user = await userService.GetByIdAsync(1);

                if (user == null)
                {
                    _logger.
                        LogError("Критическая ошибка: Пользователь с ID=1 не найден в БД!");
                    return;
                }

                CurrentUser = user;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка подключения к базе данных при загрузке пользователя: {ex}", ex);
            }
        }
    }
}