using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassManager.API.DTOs;
using PassManager.API.Services;
using PassManager.API.Services.Interfaces;
using System.Security.Claims;

namespace PassManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly TokenService _tokenService;

        public AuthController(IAuthService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var (user, errorMessage) = await _authService.Register(userForRegisterDto);

            if (user == null)
            {
                return BadRequest(errorMessage);
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _authService.Login(userForLoginDto);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _tokenService.CreateToken(user);
            return Ok(new TokenDto { Token = token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetUserId();
            var user = await _authService.GetCurrentUser(userId);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(new { user.Id, user.Username, user.Email, user.CreatedAt });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(UserForPasswordChangeDto userForPasswordChangeDto)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _authService.ChangePassword(userId, userForPasswordChangeDto);

            if (!success)
            {
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail(UserForUpdateEmailDto userForUpdateEmailDto)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _authService.UpdateEmail(userId, userForUpdateEmailDto);

            if (!success)
            {
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _authService.DeleteAccount(userId);

            if (!success)
            {
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

    }
}