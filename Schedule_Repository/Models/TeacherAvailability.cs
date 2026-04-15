using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class TeacherAvailability
{
    public long TeacherAvailabilityId { get; set; }

    public long TeacherId { get; set; }

    public DateOnly AvailableDate { get; set; }

    public long TimeSlotId { get; set; }

    public string AvailabilityStatus { get; set; } = null!;

    public string? Note { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
