using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class Semester
{
    public long SemesterId { get; set; }

    public string SemesterCode { get; set; } = null!;

    public string SemesterName { get; set; } = null!;

    public string AcademicYear { get; set; } = null!;

    public int TermNumber { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ProjectCourse> ProjectCourses { get; set; } = new List<ProjectCourse>();
}
