using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule.Controllers
{
    [Route("api/semesters")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSemesters()
        {
            var result = await _semesterService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetSemesterById(long id)
        {
            var result = await _semesterService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Không tìm thấy semester." });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] CreateSemesterRequestDto request)
        {
            var result = await _semesterService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateSemester(long id, [FromBody] UpdateSemesterRequestDto request)
        {
            var result = await _semesterService.UpdateAsync(id, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteSemester(long id)
        {
            var result = await _semesterService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
