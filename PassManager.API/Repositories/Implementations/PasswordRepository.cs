using Microsoft.EntityFrameworkCore;
using PassManager.API.Data;
using PassManager.API.Repositories.Interfaces; 
using PassManager.API.Models;

namespace PassManager.API.Repositories.Implementations 
{
    public class PasswordRepository : IPasswordRepository
    {
        private readonly ApplicationDbContext _context;

        public PasswordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Password>> GetPasswordsByGroupIdAsync(int groupId)
        {
            return await _context.Passwords
                .Where(p => p.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<Password?> GetPasswordByIdAsync(int id)
        {
            return await _context.Passwords.FindAsync(id);
        }

        public async Task AddPasswordAsync(Password password)
        {
            await _context.Passwords.AddAsync(password);
        }

        public void UpdatePassword(Password password)
        {
            _context.Entry(password).State = EntityState.Modified;
        }

        public void RemovePassword(Password password)
        {
            _context.Passwords.Remove(password);
        }

        public void RemoveRange(IEnumerable<Password> passwords)
        {
            _context.Passwords.RemoveRange(passwords);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Password>> GetPasswordsByUserIdAsync(int userId)
        {
            return await _context.Passwords
                .Include(p => p.Group)
                .Where(p => p.Group.UserId == userId)
                .ToListAsync();
        }

    }
}