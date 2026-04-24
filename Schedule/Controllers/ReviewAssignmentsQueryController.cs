using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.Service;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [ApiController]
    [Route("api/review-assignments")]
    [Authorize]
    public class ReviewAssignmentsQueryController : ControllerBase
    {
        private readonly IReviewAssignmentQueryService _reviewAssignmentQueryService;

        public ReviewAssignmentsQueryController(IReviewAssignmentQueryService reviewAssignmentQueryService)
        {
            _reviewAssignmentQueryService = reviewAssignmentQueryService;
        }

        [HttpGet("schedule-overview")]
        public async Task<IActionResult> GetScheduleOverview(
            [FromQuery] long semesterId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            [FromQuery] string? status)
        {
            var result = await _reviewAssignmentQueryService.GetScheduleOverviewAsync(
                semesterId,
                fromDate,
                toDate,
                status);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("my-schedule")]
        public async Task<IActionResult> GetMySchedule(
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
        {
            var rawUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(rawUserId, out var userId))
                return Unauthorized();

            var result = await _reviewAssignmentQueryService.GetMyScheduleAsync(userId, fromDate, toDate);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}
