using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class ProjectCourse
{
    public long ProjectCourseId { get; set; }

    public long SemesterId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ProjectGroup> ProjectGroups { get; set; } = new List<ProjectGroup>();

    public virtual Semester Semester { get; set; } = null!;
}
