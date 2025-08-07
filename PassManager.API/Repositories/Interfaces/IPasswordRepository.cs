using PassManager.API.Models;

namespace PassManager.API.Repositories.Interfaces
{
    public interface IPasswordRepository
    {
        Task<List<Password>> GetPasswordsByGroupIdAsync(int groupId);
        Task<Password?> GetPasswordByIdAsync(int id);
        Task AddPasswordAsync(Password password);
        void UpdatePassword(Password password);
        void RemovePassword(Password password);
        void RemoveRange(IEnumerable<Password> passwords);
        Task<bool> SaveAllAsync();
        Task<List<Password>> GetPasswordsByUserIdAsync(int userId);

    }
}