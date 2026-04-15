using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ReviewAssignmentTeacher
{
    public long ReviewAssignmentTeacherId { get; set; }

    public long ReviewAssignmentId { get; set; }

    public long TeacherId { get; set; }

    public string? RoleInPanel { get; set; }

    public virtual ReviewAssignment ReviewAssignment { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
