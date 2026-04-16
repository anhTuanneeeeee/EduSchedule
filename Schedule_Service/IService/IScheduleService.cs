using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schedule_Service.IService;

public interface IScheduleService
{
    Task<IEnumerable<TimeSlot>> GetTimeSlotsAsync();
    
    /// <summary>
    /// Searches for teachers available at a specific date and time slot, 
    /// excluding those who are supervisors of the specified project group.
    /// </summary>
    Task<IEnumerable<Teacher>> SearchAvailableTeachersAsync(long projectGroupId, DateOnly date, long timeSlotId);

    /// <summary>
    /// Schedules a meeting for a project group.
    /// </summary>
    Task<ReviewAssignment> ScheduleMeetingAsync(long projectGroupId, long reviewRoundId, DateOnly date, long timeSlotId, long assignedByUserId, List<long> teacherIds);

    /// <summary>
    /// Updates an existing meeting schedule.
    /// </summary>
    Task<ReviewAssignment> UpdateMeetingAsync(long reviewAssignmentId, DateOnly date, long timeSlotId, List<long> teacherIds);

    /// <summary>
    /// Cancels an existing meeting schedule.
    /// </summary>
    Task CancelMeetingAsync(long reviewAssignmentId);

    /// <summary>
    /// Gets scheduled meetings with optional filters.
    /// </summary>
    Task<IEnumerable<ReviewAssignment>> GetScheduledMeetingsAsync(long? projectGroupId = null, long? teacherId = null, string? status = null);

    /// <summary>
    /// Gets review rounds for a project group.
    /// </summary>
    Task<IEnumerable<ReviewRound>> GetReviewRoundsByGroupAsync(long projectGroupId);
}
