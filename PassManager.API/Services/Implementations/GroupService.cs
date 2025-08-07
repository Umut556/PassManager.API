using PassManager.API.DTOs;
using PassManager.API.Repositories.Interfaces;
using PassManager.API.Models;
using PassManager.API.Services.Interfaces;

namespace PassManager.API.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IPasswordRepository _passwordRepository;
        private readonly LogService _logService;

        public GroupService(IGroupRepository groupRepository, IPasswordRepository passwordRepository, LogService logService)
        {
            _groupRepository = groupRepository;
            _passwordRepository = passwordRepository;
            _logService = logService;
        }

        public async Task<List<GroupDto>> GetUserGroupsAsync(int userId)
        {
            var groups = await _groupRepository.GetGroupsByUserIdAsync(userId);
            await _logService.LogActionAsync(userId, "Tüm grupları listeledi.", true);

            return groups.Select(g => new GroupDto { Id = g.Id, GroupName = g.GroupName }).ToList();
        }

        public async Task<GroupDto?> GetUserGroupByIdAsync(int userId, int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAndUserIdAsync(groupId, userId);

            if (group == null)
            {
                await _logService.LogActionAsync(userId, $"Grup bulunamadı. Grup ID: {groupId}", false);
                return null;
            }

            await _logService.LogActionAsync(userId, $"Grup detaylarını görüntüledi. Grup ID: {groupId}", true);
            return new GroupDto { Id = group.Id, GroupName = group.GroupName };
        }

        public async Task<GroupDto?> CreateGroupAsync(int userId, CreateGroupDto dto)
        {
            var newGroup = new Group
            {
                UserId = userId,
                GroupName = dto.GroupName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _groupRepository.AddGroupAsync(newGroup);
            if (!await _groupRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Grup oluşturma başarısız: {dto.GroupName}", false);
                return null;
            }

            await _logService.LogActionAsync(userId, $"Yeni grup oluşturuldu: {newGroup.GroupName}", true);
            return new GroupDto { Id = newGroup.Id, GroupName = newGroup.GroupName };
        }

        public async Task<(bool success, string? errorMessage)> UpdateGroupAsync(int userId, int groupId, CreateGroupDto dto)
        {
            var group = await _groupRepository.GetGroupByIdAndUserIdAsync(groupId, userId);

            if (group == null)
            {
                await _logService.LogActionAsync(userId, $"Güncellenecek grup bulunamadı. Grup ID: {groupId}", false);
                return (false, "Grup bulunamadı.");
            }

            group.GroupName = dto.GroupName;
            group.UpdatedAt = DateTime.Now;

            _groupRepository.UpdateGroup(group);
            if (!await _groupRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Grup güncelleme başarısız: Grup ID: {groupId}", false);
                return (false, "Grup güncelleme işlemi başarısız.");
            }

            await _logService.LogActionAsync(userId, $"Grup güncellendi. Grup ID: {groupId}", true);
            return (true, null);
        }

        public async Task<(bool success, string? errorMessage)> DeleteGroupAsync(int userId, int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAndUserIdAsync(groupId, userId);

            if (group == null)
            {
                await _logService.LogActionAsync(userId, $"Silinecek grup bulunamadı. Grup ID: {groupId}", false);
                return (false, "Grup bulunamadı.");
            }

            var passwords = await _passwordRepository.GetPasswordsByGroupIdAsync(groupId);
            if (passwords.Any())
            {
                _passwordRepository.RemoveRange(passwords);
            }

            _groupRepository.RemoveGroup(group);

            if (!await _groupRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Grup silme başarısız: Grup ID: {groupId}", false);
                return (false, "Grup silme işlemi başarısız.");
            }

            await _logService.LogActionAsync(userId, $"Grup silindi. Grup ID: {groupId}", true);
            return (true, null);
        }
    }
}