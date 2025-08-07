using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassManager.API.DTOs;
using PassManager.API.Services.Interfaces;
using System.Security.Claims;

namespace PassManager.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public PasswordsController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim not found.");
            }
            return int.Parse(userIdClaim);
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetPasswordsByGroup(int groupId)
        {
            var userId = GetUserId();
            var passwords = await _passwordService.GetPasswordsByGroupAsync(userId, groupId);

            if (passwords == null)
            {
                return NotFound("Grup bulunamadı veya bu gruba yetkiniz yok.");
            }

            return Ok(passwords);
        }

        [HttpGet("password/{id}")]
        public async Task<IActionResult> GetPasswordById(int id)
        {
            var userId = GetUserId();
            var password = await _passwordService.GetPasswordByIdAsync(userId, id);

            if (password == null)
            {
                return NotFound("Şifre bulunamadı veya yetkiniz yok.");
            }

            return Ok(password);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword(CreatePasswordDto createPasswordDto)
        {
            var userId = GetUserId();
            var newPassword = await _passwordService.CreatePasswordAsync(userId, createPasswordDto);

            if (newPassword == null)
            {
                return Unauthorized("Bu gruba şifre ekleme yetkiniz yok.");
            }

            return CreatedAtAction(nameof(GetPasswordById), new { id = newPassword.Id }, newPassword);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassword(int id, CreatePasswordDto updatePasswordDto)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _passwordService.UpdatePasswordAsync(userId, id, updatePasswordDto);

            if (!success)
            {
                return NotFound(errorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _passwordService.DeletePasswordAsync(userId, id);

            if (!success)
            {
                return NotFound(errorMessage);
            }

            return NoContent();
        }
    }
}