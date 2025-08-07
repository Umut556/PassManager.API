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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
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

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var userId = GetUserId();
            var groups = await _groupService.GetUserGroupsAsync(userId);
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var userId = GetUserId();
            var group = await _groupService.GetUserGroupByIdAsync(userId, id);

            if (group == null)
            {
                return NotFound("Grup bulunamadı.");
            }

            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto createGroupDto)
        {
            var userId = GetUserId();
            var newGroup = await _groupService.CreateGroupAsync(userId, createGroupDto);

            if (newGroup == null)
            {
                return BadRequest("Grup oluşturma başarısız.");
            }

            return CreatedAtAction(nameof(GetGroupById), new { id = newGroup.Id }, newGroup);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, CreateGroupDto updateGroupDto)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _groupService.UpdateGroupAsync(userId, id, updateGroupDto);

            if (!success)
            {
                return NotFound(errorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var userId = GetUserId();
            var (success, errorMessage) = await _groupService.DeleteGroupAsync(userId, id);

            if (!success)
            {
                return NotFound(errorMessage);
            }

            return NoContent();
        }
    }
}