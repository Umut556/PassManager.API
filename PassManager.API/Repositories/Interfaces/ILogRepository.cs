using PassManager.API.Models;

namespace PassManager.API.Repositories.Interfaces
{
    public interface ILogRepository
    {
        Task AddLogAsync(Log log);
        Task<bool> SaveAllAsync();
    }
}