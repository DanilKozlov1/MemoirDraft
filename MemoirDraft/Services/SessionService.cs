using MemoirDraft.Database.Models;

namespace MemoirDraft.Services
{
    /// <summary>
    /// Класc-сервис для хранения текущего пользователя
    /// </summary>
    public class SessionService
    {
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private User? _currentClient;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public User? CurrentClient
        {
            get => _currentClient;
            set
            {
                if (_currentClient?.Id != value?.Id)
                {
                    _currentClient = value;
                    CurrentUserChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Ивент при изменении текущего пользователя
        /// </summary>
        public event Action? CurrentUserChanged;
    }
}