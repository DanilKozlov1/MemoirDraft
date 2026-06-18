using MemoirDraft.Database.Models;

namespace MemoirDraft.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> AuthAsync(string username, string password);
        Task CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
    }
}