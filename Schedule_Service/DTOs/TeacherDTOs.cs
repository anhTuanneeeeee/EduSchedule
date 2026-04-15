using System;
using System.Collections.Generic;

namespace Schedule_Service.DTOs;

public class AvailabilityRequest
{
    public DateOnly Date { get; set; }
    public List<long> TimeSlotIds { get; set; } = new List<long>();
    public string? Note { get; set; }
}

public class AvailabilityResponse
{
    public long AvailabilityId { get; set; }
    public long TeacherId { get; set; }
    public DateOnly Date { get; set; }
    public long TimeSlotId { get; set; }
    public string SlotName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Note { get; set; }
}

public class TimeSlotResponse
{
    public long TimeSlotId { get; set; }
    public int SlotNumber { get; set; }
    public string SlotName { get; set; } = null!;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
