using PassManager.API.Models;

namespace PassManager.API.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetGroupsByUserIdAsync(int userId);
        Task<Group?> GetGroupByIdAndUserIdAsync(int id, int userId);
        Task AddGroupAsync(Group group);
        void UpdateGroup(Group group);
        void RemoveGroup(Group group);
        void RemoveRange(IEnumerable<Group> groups);
        Task<bool> SaveAllAsync();
    }
}