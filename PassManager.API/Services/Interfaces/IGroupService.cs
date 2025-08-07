using PassManager.API.DTOs;

namespace PassManager.API.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<GroupDto>> GetUserGroupsAsync(int userId);
        Task<GroupDto?> GetUserGroupByIdAsync(int userId, int groupId);
        Task<GroupDto?> CreateGroupAsync(int userId, CreateGroupDto dto);
        Task<(bool success, string? errorMessage)> UpdateGroupAsync(int userId, int groupId, CreateGroupDto dto);
        Task<(bool success, string? errorMessage)> DeleteGroupAsync(int userId, int groupId);
    }
}