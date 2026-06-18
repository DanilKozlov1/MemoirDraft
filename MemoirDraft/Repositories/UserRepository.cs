using MemoirDraft.Database;
using MemoirDraft.Database.Models;
using MemoirDraft.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemoirDraft.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;


        public UserRepository(AppDBContext context)
        {
            _context = context;
        }


        public async Task<User?> GetByIdAsync(int userId) => await _context.Users.FindAsync(userId);

        public async Task<User?> GetByUserNameAsync(string userName) 
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) 
                return false;

            _context.Entry(existing).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) 
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}