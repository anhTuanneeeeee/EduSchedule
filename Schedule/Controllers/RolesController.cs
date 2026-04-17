using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto request)
        {
            var result = await _roleService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
    }
}
