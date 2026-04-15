using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ProjectGroup
{
    public long ProjectGroupId { get; set; }

    public long ProjectCourseId { get; set; }

    public string GroupCode { get; set; } = null!;

    public string GroupName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ProjectCourse ProjectCourse { get; set; } = null!;

    public virtual ICollection<ProjectSupervisor> ProjectSupervisors { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ReviewRound> ReviewRounds { get; set; } = new List<ReviewRound>();
}
