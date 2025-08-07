using PassManager.API.Models;

namespace PassManager.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        Task AddUserAsync(User user);
        Task<bool> SaveAllAsync();
        void Remove(User user);
    }
}