using PassManager.API.DTOs;
using PassManager.API.Models;

namespace PassManager.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(User? user, string? errorMessage)> Register(UserForRegisterDto userForRegisterDto);
        Task<User?> Login(UserForLoginDto userForLoginDto);
        Task<User?> GetCurrentUser(int userId);
        Task<(bool success, string? errorMessage)> ChangePassword(int userId, UserForPasswordChangeDto dto);
        Task<(bool success, string? errorMessage)> UpdateEmail(int userId, UserForUpdateEmailDto dto);
        Task<(bool success, string? errorMessage)> DeleteAccount(int userId);

    }
}