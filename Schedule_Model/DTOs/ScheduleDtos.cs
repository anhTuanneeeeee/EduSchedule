using System;
using System.Collections.Generic;

namespace Schedule_Model.DTOs;

/// <summary>
/// Request DTO for scheduling a new meeting.
/// </summary>
public class ScheduleMeetingRequest
{
    public long ProjectGroupId { get; set; }
    public long ReviewRoundId { get; set; }
    public DateOnly Date { get; set; }
    public long TimeSlotId { get; set; }
    public long AssignedByUserId { get; set; }
    public List<long> TeacherIds { get; set; } = new();
}

/// <summary>
/// Response DTO for teacher information.
/// </summary>
public class TeacherResponseDto
{
    public long TeacherId { get; set; }
    public string TeacherCode { get; set; } = null!;
    public string FullName { get; set; } = null!; // We'll need to join with User to get this
    public string? Department { get; set; }
    public string? Specialization { get; set; }
}

/// <summary>
/// Response DTO for time slot information.
/// </summary>
public class TimeSlotResponseDto
{
    public long TimeSlotId { get; set; }
    public string SlotName { get; set; } = null!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

/// <summary>
/// Response DTO for review round information.
/// </summary>
public class ReviewRoundResponseDto
{
    public long ReviewRoundId { get; set; }
    public long ProjectGroupId { get; set; }
    public int RoundNumber { get; set; }
    public string RoundName { get; set; } = null!;
    public DateOnly? PlannedDate { get; set; }
    public string Status { get; set; } = null!;
}

/// <summary>
/// Response DTO for a scheduled meeting assignment.
/// </summary>
public class ReviewAssignmentResponseDto
{
    public long ReviewAssignmentId { get; set; }
    public long ReviewRoundId { get; set; }
    public string? GroupCode { get; set; }
    public string? RoundName { get; set; }
    public DateOnly AssignedDate { get; set; }
    public long TimeSlotId { get; set; }
    public string? TimeSlotName { get; set; }
    public long AssignedByUserId { get; set; }
    public string? AssignedByUserName { get; set; }
    public string Status { get; set; } = null!;
    public string? Location { get; set; }
    public string? Note { get; set; }
    public List<TeacherDetailDto> Teachers { get; set; } = new();
}

/// <summary>
/// Detailed teacher information within a meeting assignment.
/// </summary>
public class TeacherDetailDto
{
    public long TeacherId { get; set; }
    public string TeacherCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
}

/// <summary>
/// Request DTO for updating an existing meeting.
/// </summary>
public class UpdateMeetingRequest
{
    public DateOnly Date { get; set; }
    public long TimeSlotId { get; set; }
    public List<long> TeacherIds { get; set; } = new();
}
