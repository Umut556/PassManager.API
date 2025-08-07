using Microsoft.EntityFrameworkCore;
using PassManager.API.Data;
using PassManager.API.DTOs;
using PassManager.API.Models;
using PassManager.API.Repositories.Interfaces;
using PassManager.API.Services.Interfaces;

namespace PassManager.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IPasswordRepository _passwordRepository;
        private readonly LogService _logService;
        private readonly ApplicationDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IPasswordRepository passwordRepository,
            LogService logService,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _passwordRepository = passwordRepository;
            _logService = logService;
            _context = context;
        }

        public async Task<(User? user, string? errorMessage)> Register(UserForRegisterDto userForRegisterDto)
        {
            if (await _userRepository.UsernameExistsAsync(userForRegisterDto.Username))
            {
                return (null, "Kullanıcı adı zaten mevcut.");
            }

            if (await _userRepository.EmailExistsAsync(userForRegisterDto.Email))
            {
                return (null, "Bu e-posta adresi zaten kayıtlı.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userForRegisterDto.Password);

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username,
                Email = userForRegisterDto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddUserAsync(userToCreate);

            if (!await _userRepository.SaveAllAsync())
            {
                return (null, "Kayıt işlemi başarısız.");
            }

            await _logService.LogActionAsync(userToCreate.Id, $"Yeni kullanıcı kaydoldu: {userToCreate.Username}", true);
            return (userToCreate, null);
        }
        public async Task<User?> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userForLoginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userForLoginDto.Password, user.PasswordHash))
            {
                if (user != null)
                {
                    await _logService.LogActionAsync(user.Id, $"Başarısız giriş denemesi: {userForLoginDto.Username}", false);
                }

                return null;
            }

            await _logService.LogActionAsync(user.Id, $"Başarılı giriş: {userForLoginDto.Username}", true);
            return user;
        }

        public async Task<User?> GetCurrentUser(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                await _logService.LogActionAsync(userId, "Kendi profil bilgilerini görüntüleme başarısız: Kullanıcı bulunamadı.", false);
                return null;
            }

            await _logService.LogActionAsync(userId, "Kendi profil bilgilerini görüntüledi.", true);
            return user;
        }

        public async Task<(bool success, string? errorMessage)> ChangePassword(int userId, UserForPasswordChangeDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                await _logService.LogActionAsync(userId, "Şifre değiştirme başarısız: Kullanıcı bulunamadı.", false);
                return (false, "Kullanıcı bulunamadı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                await _logService.LogActionAsync(userId, "Şifre değiştirme başarısız: Eski şifre yanlış.", false);
                return (false, "Eski şifre yanlış.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            if (await _userRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, "Şifresini başarıyla değiştirdi.", true);
                return (true, null);
            }

            await _logService.LogActionAsync(userId, "Şifre değiştirme başarısız: Veritabanı hatası.", false);
            return (false, "Şifre değiştirme işlemi başarısız oldu.");
        }

        public async Task<(bool success, string? errorMessage)> UpdateEmail(int userId, UserForUpdateEmailDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                await _logService.LogActionAsync(userId, "E-posta güncelleme başarısız: Kullanıcı bulunamadı.", false);
                return (false, "Kullanıcı bulunamadı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                await _logService.LogActionAsync(userId, "E-posta güncelleme başarısız: Mevcut şifre yanlış.", false);
                return (false, "Mevcut şifreniz yanlış.");
            }

            if (user.Email != dto.NewEmail && await _userRepository.UsernameExistsAsync(dto.NewEmail))
            {
                await _logService.LogActionAsync(userId, "E-posta güncelleme başarısız: E-posta adresi zaten kullanılıyor.", false);
                return (false, "Bu e-posta adresi zaten kullanılıyor.");
            }

            user.Email = dto.NewEmail;
            user.UpdatedAt = DateTime.Now;

            if (await _userRepository.SaveAllAsync())
            {
                await _logService.LogActionAsync(userId, "E-posta adresini başarıyla güncelledi.", true);
                return (true, null);
            }

            await _logService.LogActionAsync(userId, "E-posta güncelleme başarısız: Veritabanı hatası.", false);
            return (false, "E-posta güncelleme işlemi başarısız oldu.");
        }

        public async Task<(bool success, string? errorMessage)> DeleteAccount(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            var passwords = await _passwordRepository.GetPasswordsByUserIdAsync(userId);
            _context.Passwords.RemoveRange(passwords);

            var groups = await _groupRepository.GetGroupsByUserIdAsync(userId);
            _context.Groups.RemoveRange(groups);

            var logs = await _context.Logs.Where(l => l.UserId == userId).ToListAsync();
            _context.Logs.RemoveRange(logs);

            _context.Users.Remove(user);

            var saved = await _userRepository.SaveAllAsync();
            if (!saved)
                return (false, "Veritabanı işlemi sırasında hata oluştu.");

            return (true, null);
        }
    }
}