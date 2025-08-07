using PassManager.API.Data;
using PassManager.API.Models;

namespace PassManager.API.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActionAsync(int userId, string action, bool isSuccess)
        {
            var log = new Log
            {
                UserId = userId,
                Action = action,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                IsSuccess = isSuccess,
                LogDate = DateTime.Now
            };

            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}