using System;
using System.Collections.Generic;

namespace Schedule_Repository.Models;

public partial class Teacher
{
    public long TeacherId { get; set; }

    public long UserId { get; set; }

    public string TeacherCode { get; set; } = null!;

    public string? Department { get; set; }

    public string? Specialization { get; set; }

    public string? AcademicRank { get; set; }

    public bool IsAvailableForProjectReview { get; set; }

    public int MaxAssignmentsPerDay { get; set; }

    public virtual ICollection<ProjectSupervisor> ProjectSupervisors { get; set; } = new List<ProjectSupervisor>();

    public virtual ICollection<ReviewAssignmentTeacher> ReviewAssignmentTeachers { get; set; } = new List<ReviewAssignmentTeacher>();

    public virtual ICollection<TeacherAvailability> TeacherAvailabilities { get; set; } = new List<TeacherAvailability>();

    public virtual ICollection<TeacherCompatibility> TeacherCompatibilityTeacherId1Navigations { get; set; } = new List<TeacherCompatibility>();

    public virtual ICollection<TeacherCompatibility> TeacherCompatibilityTeacherId2Navigations { get; set; } = new List<TeacherCompatibility>();

    public virtual User User { get; set; } = null!;
}
