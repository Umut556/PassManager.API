using PassManager.API.DTOs;
using PassManager.API.Repositories.Interfaces;
using PassManager.API.Services.Interfaces;

namespace PassManager.API.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordRepository _passwordRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly EncryptionService _encryptionService;
        private readonly LogService _logService;

        public PasswordService(IPasswordRepository passwordRepository, IGroupRepository groupRepository, EncryptionService encryptionService, LogService logService)
        {
            _passwordRepository = passwordRepository;
            _groupRepository = groupRepository;
            _encryptionService = encryptionService;
            _logService = logService;
        }

        public async Task<List<PasswordDto>?> GetPasswordsByGroupAsync(int userId, int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAndUserIdAsync(groupId, userId);
            if (group == null)
            {
                await _logService.LogActionAsync(userId, $"Gruba ait şifreler listelenemedi. Grup bulunamadı veya yetkisiz erişim. Grup ID: {groupId}", false);
                return null;
            }

            var passwords = await _passwordRepository.GetPasswordsByGroupIdAsync(groupId);
            await _logService.LogActionAsync(userId, $"Gruptaki şifreler listelendi. Grup ID: {groupId}", true);

            return passwords.Select(p => new PasswordDto
            {
                Id = p.Id,
                GroupId = p.GroupId,
                Title = p.Title,
                EncryptedPassword = _encryptionService.Decrypt(p.EncryptedPassword),
                Username = p.Username,
                URL = p.URL
            }).ToList();
        }

        public async Task<PasswordDto?> GetPasswordByIdAsync(int userId, int passwordId)
        {
            var password = await _passwordRepository.GetPasswordByIdAsync(passwordId);
            if (password == null)
            {
                await _logService.LogActionAsync(userId, $"Şifre bulunamadı. Şifre ID: {passwordId}", false);
                return null;
            }
            if ((await _groupRepository.GetGroupByIdAndUserIdAsync(password.GroupId, userId)) == null)
            {
                await _logService.LogActionAsync(userId, $"Şifreye yetkisiz erişim. Şifre ID: {passwordId}", false);
                return null;
            }

            await _logService.LogActionAsync(userId, $"Şifre detayları görüntülendi. Şifre ID: {passwordId}", true);
            return new PasswordDto
            {
                Id = password.Id,
                GroupId = password.GroupId,
                Title = password.Title,
                EncryptedPassword = _encryptionService.Decrypt(password.EncryptedPassword),
                Username = password.Username,
                URL = password.URL
            };
        }

        public async Task<PasswordDto?> CreatePasswordAsync(int userId, CreatePasswordDto dto)
        {
            var group = await _groupRepository.GetGroupByIdAndUserIdAsync(dto.GroupId, userId);
            if (group == null)
            {
                await _logService.LogActionAsync(userId, $"Şifre eklenemedi. Grup bulunamadı veya yetkisiz erişim. Grup ID: {dto.GroupId}", false);
                return null;
            }

            var newPassword = new Models.Password
            {
                GroupId = dto.GroupId,
                Title = dto.Title,
                EncryptedPassword = _encryptionService.Encrypt(dto.EncryptedPassword),
                Username = dto.Username,
                URL = dto.URL,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _passwordRepository.AddPasswordAsync(newPassword);
            if (!await _passwordRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Şifre oluşturma başarısız: {newPassword.Title}", false);
                return null;
            }

            await _logService.LogActionAsync(userId, $"Yeni şifre eklendi. Şifre Başlığı: {newPassword.Title}", true);
            return new PasswordDto
            {
                Id = newPassword.Id,
                GroupId = newPassword.GroupId,
                Title = newPassword.Title,
                EncryptedPassword = _encryptionService.Decrypt(newPassword.EncryptedPassword),
                Username = newPassword.Username,
                URL = newPassword.URL
            };
        }

        public async Task<(bool success, string? errorMessage)> UpdatePasswordAsync(int userId, int passwordId, CreatePasswordDto dto)
        {
            var password = await _passwordRepository.GetPasswordByIdAsync(passwordId);
            if (password == null)
            {
                await _logService.LogActionAsync(userId, $"Güncellenecek şifre bulunamadı. Şifre ID: {passwordId}", false);
                return (false, "Şifre bulunamadı.");
            }
            if ((await _groupRepository.GetGroupByIdAndUserIdAsync(password.GroupId, userId)) == null)
            {
                await _logService.LogActionAsync(userId, $"Şifreye yetkisiz erişim. Şifre ID: {passwordId}", false);
                return (false, "Bu şifreyi güncellemeye yetkiniz yok.");
            }

            password.GroupId = dto.GroupId;
            password.Title = dto.Title;
            password.EncryptedPassword = _encryptionService.Encrypt(dto.EncryptedPassword);
            password.Username = dto.Username;
            password.URL = dto.URL;
            password.UpdatedAt = DateTime.Now;

            _passwordRepository.UpdatePassword(password);
            if (!await _passwordRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Şifre güncelleme başarısız: Şifre ID: {passwordId}", false);
                return (false, "Şifre güncelleme işlemi başarısız.");
            }

            await _logService.LogActionAsync(userId, $"Şifre güncellendi. Şifre ID: {passwordId}", true);
            return (true, null);
        }

        public async Task<(bool success, string? errorMessage)> DeletePasswordAsync(int userId, int passwordId)
        {
            var password = await _passwordRepository.GetPasswordByIdAsync(passwordId);
            if (password == null)
            {
                await _logService.LogActionAsync(userId, $"Silinecek şifre bulunamadı. Şifre ID: {passwordId}", false);
                return (false, "Şifre bulunamadı.");
            }
            if ((await _groupRepository.GetGroupByIdAndUserIdAsync(password.GroupId, userId)) == null)
            {
                await _logService.LogActionAsync(userId, $"Şifreye yetkisiz erişim. Şifre ID: {passwordId}", false);
                return (false, "Bu şifreyi silmeye yetkiniz yok.");
            }

            _passwordRepository.RemovePassword(password);
            if (!await _passwordRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, $"Şifre silme başarısız: Şifre ID: {passwordId}", false);
                return (false, "Şifre silme işlemi başarısız.");
            }

            await _logService.LogActionAsync(userId, $"Şifre silindi. Şifre ID: {passwordId}", true);
            return (true, null);
        }
    }
}