using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using MemoirDraft.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MemoirDraft.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;


        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос на получение пользователя по id={userId}.", id);
            try
            {
                User? user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    _logger.LogWarning("Пользователь по данному id({userId}) не найден или не существует.", id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            _logger.LogInformation("Запрос на получение пользователя по UserName={userName}.", userName);
            try
            {
                User? user = await _userRepository.GetByUserNameAsync(userName);
                if (user == null)
                    _logger.LogWarning("Пользователь по данному Username({userName}) не найден или не существует.", userName);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса к базе данных: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<User?> AuthAsync(string username, string password)
        {
            _logger.LogInformation("Попытка входа пользователя {UserName}", username);
            try
            {
                var user = await _userRepository.GetByUserNameAsync(username);

                if (user == null)
                {
                    _logger.LogWarning("Пользователь {UserName} не найден", username);
                    return null;
                }
                if (user.Password != password)
                {
                    _logger.LogWarning("Неверный пароль для пользователя {UserName}", username);
                    return null;
                }

                _logger.LogInformation("Пользователь {UserName} успешно вошёл", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при аутентификации пользователя {UserName}", username);
                throw;
            }
        }

        public async Task CreateAsync(User user)
        {
            _logger.LogInformation("Создание нового пользователя.");
            try
            {
                await _userRepository.AddAsync(user);
                _logger.LogInformation("Новый пользователь успешно создан.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при создании пользователя: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _logger.LogInformation("Обновление пользователя {userName}.", user.Username);
            try
            {
                bool success = await _userRepository.UpdateAsync(user);

                if (success)
                    _logger.LogInformation("Пользователь {userName} обновлён.", user.Username);
                else
                    _logger.LogWarning("Пользователь {userName} не был найден.", user.Username);

                return success;
            }
            catch (Exception ex) 
            {
                _logger.LogError("Ошибка при обновлении пользователя: {errorMessage}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            _logger.LogInformation("Удаление пользователя по id={userId}.", userId);
            try
            {
                bool success = await _userRepository.DeleteAsync(userId);

                if (success)
                    _logger.LogInformation("Пользователь по id={userId} удалён.", userId);
                else
                    _logger.LogWarning("Пользователь по id={userId} не был найден.", userId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при удалении пользователя: {errorMessage}", ex);
                throw;
            }
        }
    }
}