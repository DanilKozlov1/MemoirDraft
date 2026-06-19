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
        private User? _currentUser;

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
    }
}