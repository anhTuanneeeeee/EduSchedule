using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ProjectSupervisor
{
    public long ProjectSupervisorId { get; set; }

    public long ProjectGroupId { get; set; }

    public long TeacherId { get; set; }

    public int SupervisorOrder { get; set; }

    public string? RoleName { get; set; }

    public virtual ProjectGroup ProjectGroup { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
