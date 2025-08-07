using PassManager.API.Data;
using PassManager.API.Repositories.Interfaces; 
using PassManager.API.Models;

namespace PassManager.API.Repositories.Implementations 
{
    public class LogRepository : ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}