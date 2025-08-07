using Microsoft.EntityFrameworkCore;
using PassManager.API.Data;
using PassManager.API.Repositories.Interfaces; 
using PassManager.API.Models;

namespace PassManager.API.Repositories.Implementations 
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.Groups
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAndUserIdAsync(int id, int userId)
        {
            return await _context.Groups.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        }

        public async Task AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }

        public void UpdateGroup(Group group)
        {
            _context.Entry(group).State = EntityState.Modified;
        }

        public void RemoveGroup(Group group)
        {
            _context.Groups.Remove(group);
        }

        public void RemoveRange(IEnumerable<Group> groups)
        {
            _context.Groups.RemoveRange(groups);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}