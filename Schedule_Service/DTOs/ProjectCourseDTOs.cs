using System;
using System.Collections.Generic;

namespace Schedule_Service.DTOs
{
    public class ProjectCourseResponseDto
    {
        public long ProjectCourseId { get; set; }
        public long SemesterId { get; set; }
        public string CourseCode { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateProjectCourseRequestDto
    {
        public long SemesterId { get; set; }
        public string CourseCode { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class UpdateProjectCourseRequestDto
    {
        public string CourseCode { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
