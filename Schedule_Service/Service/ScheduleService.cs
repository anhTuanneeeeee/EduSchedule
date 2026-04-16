using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using Schedule_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule_Service.Service;

public class ScheduleService : IScheduleService
{
    private readonly ScheduleForTeacherContext _context;

    public ScheduleService(ScheduleForTeacherContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsAsync()
    {
        return await _context.TimeSlots.OrderBy(ts => ts.StartTime).ToListAsync();
    }

    public async Task<IEnumerable<Teacher>> SearchAvailableTeachersAsync(long projectGroupId, DateOnly date, long timeSlotId)
    {
        var supervisorTeacherIds = await _context.ProjectSupervisors
            .Where(ps => ps.ProjectGroupId == projectGroupId)
            .Select(ps => ps.TeacherId)
            .ToListAsync();

        // 2. Find teachers who have availability for the given date and time slot
        // and are NOT supervisors of the project group
        var availableTeachers = await _context.Teachers
            .Include(t => t.User)
            .Where(t => !supervisorTeacherIds.Contains(t.TeacherId)
                        && _context.TeacherAvailabilities.Any(ta => 
                            ta.TeacherId == t.TeacherId 
                            && ta.AvailableDate == date 
                            && ta.TimeSlotId == timeSlotId 
                            && ta.AvailabilityStatus == "AVAILABLE"))
            .ToListAsync();

        return availableTeachers;
    }

    public async Task<ReviewAssignment> ScheduleMeetingAsync(long projectGroupId, long reviewRoundId, DateOnly date, long timeSlotId, long assignedByUserId, List<long> teacherIds)
    {
        // 1. Validation: Ensure no teacher is a supervisor of the group
        var supervisorTeacherIds = await GetSupervisorTeacherIdsAsync(projectGroupId);
        ValidateNotSupervisors(teacherIds, supervisorTeacherIds);

        // 2. Validation: Ensure the review round exists and belongs to the project group
        var reviewRound = await _context.ReviewRounds
            .FirstOrDefaultAsync(rr => rr.ReviewRoundId == reviewRoundId && rr.ProjectGroupId == projectGroupId);
        
        if (reviewRound == null)
        {
            throw new ArgumentException("Invalid review round or project group.");
        }

        // 3. Validation: Ensure the review round doesn't already have an assignment
        var existingAssignment = await _context.ReviewAssignments
            .AnyAsync(ra => ra.ReviewRoundId == reviewRoundId);
        
        if (existingAssignment)
        {
            throw new InvalidOperationException("This review round already has a scheduled meeting.");
        }

        // 4. Validation: Ensure assignedByUserId exists
        var userExists = await _context.Users.AnyAsync(u => u.UserId == assignedByUserId);
        if (!userExists)
        {
            throw new ArgumentException($"User with ID {assignedByUserId} does not exist.");
        }

        // 5. Validation: Ensure timeSlotId exists
        var slotExists = await _context.TimeSlots.AnyAsync(ts => ts.TimeSlotId == timeSlotId);
        if (!slotExists)
        {
            throw new ArgumentException($"Time slot with ID {timeSlotId} does not exist.");
        }

        // 6. Validation: Ensure all teacherIds exist
        await ValidateTeachersExistAsync(teacherIds);

        // 6.5. Validation: Ensure all teachers are available at the given date and time slot
        await ValidateTeachersAvailableAsync(teacherIds, date, timeSlotId);

        // 6.6. Validation: Ensure all teachers are not already assigned to another meeting at the same time
        await ValidateTeachersNotBusyAsync(teacherIds, date, timeSlotId);

        // 7. Create the ReviewAssignment
        var assignment = new ReviewAssignment
        {
            ReviewRoundId = reviewRoundId,
            AssignedDate = date,
            TimeSlotId = timeSlotId,
            AssignedByUserId = assignedByUserId,
            Status = "CONFIRMED",
            ReviewAssignmentTeachers = teacherIds.Distinct().Select(tid => new ReviewAssignmentTeacher
            {
                TeacherId = tid,
                RoleInPanel = "MEMBER"
            }).ToList()
        };

        // 8. Update the ReviewRound status
        reviewRound.Status = "CONFIRMED";

        _context.ReviewAssignments.Add(assignment);

        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log or wrap the exception for better debugging
            throw new InvalidOperationException("Failed to save the meeting schedule. Please check if this review round already has an assignment or if the data is valid.", ex);
        }

        return assignment;
    }

    public async Task<ReviewAssignment> UpdateMeetingAsync(long reviewAssignmentId, DateOnly date, long timeSlotId, List<long> teacherIds)
    {
        // 1. Get the existing assignment
        var assignment = await _context.ReviewAssignments
            .Include(ra => ra.ReviewAssignmentTeachers)
            .Include(ra => ra.ReviewRound)
            .FirstOrDefaultAsync(ra => ra.ReviewAssignmentId == reviewAssignmentId);

        if (assignment == null)
        {
            throw new ArgumentException($"Review assignment with ID {reviewAssignmentId} not found.");
        }

        // 2. Validation: Ensure no teacher is a supervisor of the group
        var supervisorTeacherIds = await GetSupervisorTeacherIdsAsync(assignment.ReviewRound.ProjectGroupId);
        ValidateNotSupervisors(teacherIds, supervisorTeacherIds);

        // 3. Validation: Ensure timeSlotId exists
        var slotExists = await _context.TimeSlots.AnyAsync(ts => ts.TimeSlotId == timeSlotId);
        if (!slotExists)
        {
            throw new ArgumentException($"Time slot with ID {timeSlotId} does not exist.");
        }

        // 4. Validation: Ensure all teacherIds exist
        await ValidateTeachersExistAsync(teacherIds);

        // 5. Validation: Ensure all teachers are available at the given date and time slot
        await ValidateTeachersAvailableAsync(teacherIds, date, timeSlotId);

        // 6. Validation: Ensure all teachers are not already assigned to another meeting at the same time
        // Note: We need to exclude the current assignment when checking for busy teachers
        await ValidateTeachersNotBusyAsync(teacherIds, date, timeSlotId, reviewAssignmentId);

        // 7. Update fields
        assignment.AssignedDate = date;
        assignment.TimeSlotId = timeSlotId;

        // 8. Update teachers: Remove old ones, add new ones
        _context.ReviewAssignmentTeachers.RemoveRange(assignment.ReviewAssignmentTeachers);
        
        foreach (var teacherId in teacherIds.Distinct())
        {
            assignment.ReviewAssignmentTeachers.Add(new ReviewAssignmentTeacher
            {
                ReviewAssignmentId = reviewAssignmentId,
                TeacherId = teacherId,
                RoleInPanel = "MEMBER"
            });
        }

        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Failed to update the meeting schedule.", ex);
        }

        return assignment;
    }

    public async Task CancelMeetingAsync(long reviewAssignmentId)
    {
        var assignment = await _context.ReviewAssignments
            .Include(ra => ra.ReviewRound)
            .FirstOrDefaultAsync(ra => ra.ReviewAssignmentId == reviewAssignmentId);

        if (assignment == null)
        {
            throw new ArgumentException($"Review assignment with ID {reviewAssignmentId} not found.");
        }

        // Update status to CANCELLED instead of hard deleting (per database constraint)
        assignment.Status = "CANCELLED";
        
        // Also update the review round status back to PENDING or CANCELLED
        assignment.ReviewRound.Status = "CANCELLED";

        await _context.SaveChangesAsync();
    }

    private async Task<List<long>> GetSupervisorTeacherIdsAsync(long projectGroupId)
    {
        return await _context.ProjectSupervisors
            .Where(ps => ps.ProjectGroupId == projectGroupId)
            .Select(ps => ps.TeacherId)
            .ToListAsync();
    }

    private void ValidateNotSupervisors(List<long> teacherIds, List<long> supervisorTeacherIds)
    {
        if (teacherIds.Any(tid => supervisorTeacherIds.Contains(tid)))
        {
            throw new InvalidOperationException("One or more assigned teachers are supervisors of this project group.");
        }
    }

    private async Task ValidateTeachersExistAsync(List<long> teacherIds)
    {
        var existingCount = await _context.Teachers
            .CountAsync(t => teacherIds.Contains(t.TeacherId));
        
        if (existingCount != teacherIds.Distinct().Count())
        {
            throw new ArgumentException("One or more teacher IDs are invalid.");
        }
    }

    private async Task ValidateTeachersAvailableAsync(List<long> teacherIds, DateOnly date, long timeSlotId)
    {
        var unavailableTeachers = await _context.Teachers
            .Where(t => teacherIds.Contains(t.TeacherId) && 
                        !_context.TeacherAvailabilities.Any(ta => 
                            ta.TeacherId == t.TeacherId && 
                            ta.AvailableDate == date && 
                            ta.TimeSlotId == timeSlotId && 
                            ta.AvailabilityStatus == "AVAILABLE"))
            .Select(t => t.TeacherCode)
            .ToListAsync();

        if (unavailableTeachers.Any())
        {
            throw new InvalidOperationException($"The following teachers are not available in their schedule at the selected time: {string.Join(", ", unavailableTeachers)}");
        }
    }

    private async Task ValidateTeachersNotBusyAsync(List<long> teacherIds, DateOnly date, long timeSlotId, long? excludeAssignmentId = null)
    {
        var busyTeachersQuery = _context.ReviewAssignmentTeachers
            .Include(rat => rat.ReviewAssignment)
            .Where(rat => teacherIds.Contains(rat.TeacherId) && 
                         rat.ReviewAssignment.AssignedDate == date && 
                         rat.ReviewAssignment.TimeSlotId == timeSlotId &&
                         rat.ReviewAssignment.Status != "CANCELLED");

        if (excludeAssignmentId.HasValue)
        {
            busyTeachersQuery = busyTeachersQuery.Where(rat => rat.ReviewAssignmentId != excludeAssignmentId.Value);
        }

        var busyTeachers = await busyTeachersQuery
            .Select(rat => rat.Teacher.TeacherCode)
            .ToListAsync();

        if (busyTeachers.Any())
        {
            throw new InvalidOperationException($"The following teachers are already assigned to another review meeting at this time: {string.Join(", ", busyTeachers.Distinct())}");
        }
    }

    public async Task<IEnumerable<ReviewRound>> GetReviewRoundsByGroupAsync(long projectGroupId)
    {
        return await _context.ReviewRounds
            .Where(rr => rr.ProjectGroupId == projectGroupId)
            .OrderBy(rr => rr.RoundNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<ReviewAssignment>> GetScheduledMeetingsAsync(long? projectGroupId = null, long? teacherId = null, string? status = null)
    {
        var query = _context.ReviewAssignments
            .Include(ra => ra.ReviewRound)
                .ThenInclude(rr => rr.ProjectGroup)
            .Include(ra => ra.TimeSlot)
            .Include(ra => ra.AssignedByUser)
            .Include(ra => ra.ReviewAssignmentTeachers)
                .ThenInclude(rat => rat.Teacher)
                    .ThenInclude(t => t.User)
            .AsQueryable();

        if (projectGroupId.HasValue)
        {
            query = query.Where(ra => ra.ReviewRound.ProjectGroupId == projectGroupId.Value);
        }

        if (teacherId.HasValue)
        {
            query = query.Where(ra => ra.ReviewAssignmentTeachers.Any(rat => rat.TeacherId == teacherId.Value));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(ra => ra.Status == status);
        }

        return await query.OrderByDescending(ra => ra.AssignedDate).ThenBy(ra => ra.TimeSlot.StartTime).ToListAsync();
    }
}
