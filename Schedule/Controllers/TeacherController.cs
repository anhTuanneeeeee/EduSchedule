using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_Service.DTOs;
using Schedule_Service.IService;
using System.Security.Claims;

namespace Schedule.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet("timeslots")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTimeSlots()
    {
        var result = await _teacherService.GetTimeSlotsAsync();
        return Ok(result);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability([FromQuery] DateOnly date)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _teacherService.GetTeacherAvailabilityAsync(userId.Value, date);
        return Ok(result);
    }

    [HttpPost("availability")]
    public async Task<IActionResult> SubmitAvailability([FromBody] AvailabilityRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _teacherService.SubmitAvailabilityAsync(userId.Value, request);
        if (!result) return BadRequest("Teacher profile not found for this user.");
        
        return Ok("Availability saved successfully.");
    }

    private long? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        {
            return userId;
        }
        return null;
    }
}
