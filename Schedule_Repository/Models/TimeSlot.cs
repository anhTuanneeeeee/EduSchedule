using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class TimeSlot
{
    public long TimeSlotId { get; set; }

    public int SlotNumber { get; set; }

    public string SlotName { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ReviewAssignment> ReviewAssignments { get; set; } = new List<ReviewAssignment>();

    public virtual ICollection<TeacherAvailability> TeacherAvailabilities { get; set; } = new List<TeacherAvailability>();
}
