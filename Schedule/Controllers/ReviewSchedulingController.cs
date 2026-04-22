using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.Service;

namespace Schedule.Controllers
{
    [ApiController]
    [Route("api/review-assignments")]
    [Authorize]
    public class ReviewSchedulingController : ControllerBase
    {
        private readonly IReviewSchedulingService _reviewSchedulingService;

        public ReviewSchedulingController(IReviewSchedulingService reviewSchedulingService)
        {
            _reviewSchedulingService = reviewSchedulingService;
        }

        [HttpPost("auto-schedule")]
        public async Task<IActionResult> AutoSchedule([FromBody] AutoScheduleReviewAssignmentsRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewSchedulingService.AutoScheduleAsync(userId.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        [HttpPost("manual-schedule")]
        public async Task<IActionResult> ManualSchedule([FromBody] ManualScheduleReviewAssignmentRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewSchedulingService.ManualScheduleAsync(userId.Value, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
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