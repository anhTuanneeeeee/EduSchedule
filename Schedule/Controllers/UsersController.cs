using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy user." });

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _userService.UpdateAsync(id, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRolesByUserId(int id)
        {
            var result = await _userService.GetRolesByUserIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy user." });

            return Ok(result);
        }

        [HttpPost("{id}/roles/{roleId}")]
        public async Task<IActionResult> AssignRole(int id, int roleId)
        {
            var result = await _userService.AssignRoleAsync(id, roleId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRole(int id, int roleId)
        {
            var result = await _userService.RemoveRoleAsync(id, roleId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetUsersByRoleName(string roleName)
        {
            var result = await _userService.GetUsersByRoleNameAsync(roleName);
            return Ok(result);
        }
    }
}
