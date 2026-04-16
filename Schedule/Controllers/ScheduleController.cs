using Microsoft.AspNetCore.Mvc;
using Schedule_Model.DTOs;
using Schedule_Repository.Models;
using Schedule_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Controllers;

/// <summary>
/// Controller for managing meeting schedules.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// Gets all available time slots for scheduling.
    /// </summary>
    [HttpGet("timeslots")]
    public async Task<ActionResult<IEnumerable<TimeSlotResponseDto>>> GetTimeSlots()
    {
        var slots = await _scheduleService.GetTimeSlotsAsync();
        return Ok(slots.Select(s => new TimeSlotResponseDto
        {
            TimeSlotId = s.TimeSlotId,
            SlotName = s.SlotName,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }));
    }

    /// <summary>
    /// Gets review rounds for a project group.
    /// </summary>
    [HttpGet("review-rounds/{projectGroupId}")]
    public async Task<ActionResult<IEnumerable<ReviewRoundResponseDto>>> GetReviewRounds(long projectGroupId)
    {
        var rounds = await _scheduleService.GetReviewRoundsByGroupAsync(projectGroupId);
        return Ok(rounds.Select(r => new ReviewRoundResponseDto
        {
            ReviewRoundId = r.ReviewRoundId,
            ProjectGroupId = r.ProjectGroupId,
            RoundNumber = r.RoundNumber,
            RoundName = r.RoundName,
            PlannedDate = r.PlannedDate,
            Status = r.Status
        }));
    }

    /// <summary>
    /// Searches for available teachers at a specific date and time slot.
    /// Filters out teachers who are supervisors of the specified group.
    /// </summary>
    [HttpGet("available-teachers")]
    public async Task<ActionResult<IEnumerable<TeacherResponseDto>>> GetAvailableTeachers(
        [FromQuery] long projectGroupId, 
        [FromQuery] DateOnly date, 
        [FromQuery] long timeSlotId)
    {
        var teachers = await _scheduleService.SearchAvailableTeachersAsync(projectGroupId, date, timeSlotId);
        

        return Ok(teachers.Select(t => new TeacherResponseDto
        {
            TeacherId = t.TeacherId,
            TeacherCode = t.TeacherCode,
            FullName = t.User?.FullName ?? "Unknown",
            Department = t.Department,
            Specialization = t.Specialization
        }));
    }

    /// <summary>
    /// Gets all scheduled meetings with optional filtering.
    /// </summary>
    [HttpGet("meetings")]
    public async Task<ActionResult<IEnumerable<ReviewAssignmentResponseDto>>> GetScheduledMeetings(
        [FromQuery] long? projectGroupId = null,
        [FromQuery] long? teacherId = null,
        [FromQuery] string? status = null)
    {
        var meetings = await _scheduleService.GetScheduledMeetingsAsync(projectGroupId, teacherId, status);
        return Ok(meetings.Select(MapToDto));
    }

    /// <summary>
    /// Schedules a new meeting.
    /// </summary>
    [HttpPost("meeting")]
    public async Task<IActionResult> ScheduleMeeting([FromBody] ScheduleMeetingRequest request)
    {
        try
        {
            var assignment = await _scheduleService.ScheduleMeetingAsync(
                request.ProjectGroupId,
                request.ReviewRoundId,
                request.Date,
                request.TimeSlotId,
                request.AssignedByUserId,
                request.TeacherIds);

            var response = MapToDto(assignment);

            return CreatedAtAction(nameof(ScheduleMeeting), new { id = response.ReviewAssignmentId }, response);
        }
        catch (InvalidOperationException ex)
        {
            var message = ex.Message;
            var inner = ex.InnerException;
            while (inner != null)
            {
                message += $" | Inner: {inner.Message}";
                inner = inner.InnerException;
            }
            return BadRequest(message);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing meeting schedule.
    /// </summary>
    [HttpPut("meeting/{id}")]
    public async Task<IActionResult> UpdateMeeting(long id, [FromBody] UpdateMeetingRequest request)
    {
        try
        {
            var assignment = await _scheduleService.UpdateMeetingAsync(
                id,
                request.Date,
                request.TimeSlotId,
                request.TeacherIds);

            var response = MapToDto(assignment);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancels a meeting schedule (Soft delete).
    /// </summary>
    [HttpDelete("meeting/{id}")]
    public async Task<IActionResult> CancelMeeting(long id)
    {
        try
        {
            await _scheduleService.CancelMeetingAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    private ReviewAssignmentResponseDto MapToDto(ReviewAssignment assignment)
    {
        return new ReviewAssignmentResponseDto
        {
            ReviewAssignmentId = assignment.ReviewAssignmentId,
            ReviewRoundId = assignment.ReviewRoundId,
            GroupCode = assignment.ReviewRound?.ProjectGroup?.GroupCode,
            RoundName = assignment.ReviewRound?.RoundName,
            AssignedDate = assignment.AssignedDate,
            TimeSlotId = assignment.TimeSlotId,
            TimeSlotName = assignment.TimeSlot?.SlotName,
            AssignedByUserId = assignment.AssignedByUserId,
            AssignedByUserName = assignment.AssignedByUser?.FullName,
            Status = assignment.Status,
            Location = assignment.Location,
            Note = assignment.Note,
            Teachers = assignment.ReviewAssignmentTeachers.Select(rat => new TeacherDetailDto
            {
                TeacherId = rat.TeacherId,
                TeacherCode = rat.Teacher?.TeacherCode ?? "N/A",
                FullName = rat.Teacher?.User?.FullName ?? "Unknown"
            }).ToList()
        };
    }
}
