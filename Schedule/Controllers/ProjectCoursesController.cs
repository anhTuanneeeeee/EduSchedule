using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule.Controllers
{
    [Route("api/project-courses")]
    [ApiController]
    public class ProjectCoursesController : ControllerBase
    {
        private readonly IProjectCourseService _projectCourseService;

        public ProjectCoursesController(IProjectCourseService projectCourseService)
        {
            _projectCourseService = projectCourseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjectCourses()
        {
            var result = await _projectCourseService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProjectCourseById(long id)
        {
            var result = await _projectCourseService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy ProjectCourse." });
            return Ok(result);
        }

        [HttpGet("semester/{semesterId:long}")]
        public async Task<IActionResult> GetProjectCoursesBySemester(long semesterId)
        {
            var result = await _projectCourseService.GetBySemesterIdAsync(semesterId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectCourse([FromBody] CreateProjectCourseRequestDto request)
        {
            var result = await _projectCourseService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message, data = result.Data });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProjectCourse(long id, [FromBody] UpdateProjectCourseRequestDto request)
        {
            var result = await _projectCourseService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProjectCourse(long id)
        {
            var result = await _projectCourseService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
