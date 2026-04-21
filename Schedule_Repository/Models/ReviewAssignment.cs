using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ReviewAssignment
{
    public long ReviewAssignmentId { get; set; }

    public long ReviewRoundId { get; set; }

    public long AssignedByUserId { get; set; }

    public DateOnly AssignedDate { get; set; }

    public long TimeSlotId { get; set; }

    public string? Location { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public virtual User AssignedByUser { get; set; } = null!;

    public virtual ICollection<ReviewAssignmentTeacher> ReviewAssignmentTeachers { get; set; } = new List<ReviewAssignmentTeacher>();

    public virtual ReviewRound ReviewRound { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
