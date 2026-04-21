using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class TeacherCompatibility
{
    public long TeacherCompatibilityId { get; set; }

    public long TeacherId1 { get; set; }

    public long TeacherId2 { get; set; }

    public string PreferenceType { get; set; } = null!;

    public string? Note { get; set; }

    public virtual Teacher TeacherId1Navigation { get; set; } = null!;

    public virtual Teacher TeacherId2Navigation { get; set; } = null!;
}
