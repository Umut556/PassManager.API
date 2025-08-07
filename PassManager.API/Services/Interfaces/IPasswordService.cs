using PassManager.API.DTOs;

namespace PassManager.API.Services.Interfaces
{
    public interface IPasswordService
    {
        Task<List<PasswordDto>?> GetPasswordsByGroupAsync(int userId, int groupId);
        Task<PasswordDto?> GetPasswordByIdAsync(int userId, int passwordId);
        Task<PasswordDto?> CreatePasswordAsync(int userId, CreatePasswordDto dto);
        Task<(bool success, string? errorMessage)> UpdatePasswordAsync(int userId, int passwordId, CreatePasswordDto dto);
        Task<(bool success, string? errorMessage)> DeletePasswordAsync(int userId, int passwordId);
    }
}