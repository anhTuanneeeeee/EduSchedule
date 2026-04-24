using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.Service;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [ApiController]
    [Route("api/review-assignments")]
    [Authorize]
    public class ReviewAutoSchedulingController : ControllerBase
    {
        private readonly IReviewAutoSchedulingService _reviewAutoSchedulingService;

        public ReviewAutoSchedulingController(IReviewAutoSchedulingService reviewAutoSchedulingService)
        {
            _reviewAutoSchedulingService = reviewAutoSchedulingService;
        }

        [HttpPost("auto-schedules")]
        public async Task<IActionResult> AutoSchedule([FromBody] AutoScheduleBySemesterRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewAutoSchedulingService.AutoScheduleBySemesterAsync(userId.Value, request);

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
