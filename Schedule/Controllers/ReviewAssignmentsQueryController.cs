using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.Service;

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
    }
}
