using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule.Controllers
{
    [Route("api/project-groups")]
    [ApiController]
    public class ProjectGroupsController : ControllerBase
    {
        private readonly IProjectGroupService _projectGroupService;

        public ProjectGroupsController(IProjectGroupService projectGroupService)
        {
            _projectGroupService = projectGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectGroups()
        {
            var result = await _projectGroupService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProjectGroupById(long id)
        {
            var result = await _projectGroupService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy ProjectGroup." });
            return Ok(result);
        }

        [HttpGet("course/{courseId:long}")]
        public async Task<IActionResult> GetProjectGroupsByCourse(long courseId)
        {
            var result = await _projectGroupService.GetByProjectCourseIdAsync(courseId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectGroup([FromBody] CreateProjectGroupRequestDto request)
        {
            var result = await _projectGroupService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProjectGroup(long id, [FromBody] UpdateProjectGroupRequestDto request)
        {
            var result = await _projectGroupService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProjectGroup(long id)
        {
            var result = await _projectGroupService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
