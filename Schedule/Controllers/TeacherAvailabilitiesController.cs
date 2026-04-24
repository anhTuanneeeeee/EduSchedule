using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAvailabilitiesController : ControllerBase
    {
        private readonly ITeacherAvailabilityService _teacherAvailabilityService;

        public TeacherAvailabilitiesController(ITeacherAvailabilityService teacherAvailabilityService)
        {
            _teacherAvailabilityService = teacherAvailabilityService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyAvailabilities(
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _teacherAvailabilityService.GetMyAsync(
                userId.Value,
                fromDate,
                toDate);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }
        [HttpGet("teacher-code/{teacherCode}")]
        public async Task<IActionResult> GetByTeacherCode(
    string teacherCode,
    [FromQuery] DateOnly? fromDate,
    [FromQuery] DateOnly? toDate)
        {
            var result = await _teacherAvailabilityService.GetByTeacherCodeAsync(
                teacherCode,
                fromDate,
                toDate);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherAvailabilityRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _teacherAvailabilityService.CreateAsync(userId.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTeacherAvailabilityRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _teacherAvailabilityService.UpdateAsync(userId.Value, id, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("available-on")]
        public async Task<IActionResult> GetAvailableTeachersOnSlot(
            [FromQuery] DateOnly date,
            [FromQuery] long slotId)
        {
            var result = await _teacherAvailabilityService.GetAvailableOnSlotAsync(date, slotId);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _teacherAvailabilityService.DeleteAsync(userId.Value, id);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        private long? GetCurrentUserId()
        {
            var rawUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("userId")?.Value
                         ?? User.FindFirst("sub")?.Value;

            return long.TryParse(rawUserId, out var userId) ? userId : null;
        }
    }
}
