using System;
using System.Collections.Generic;

namespace Schedule_Service.DTOs
{
    public class ProjectGroupResponseDto
    {
        public long ProjectGroupId { get; set; }
        public long ProjectCourseId { get; set; }
        public string GroupCode { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class CreateProjectGroupRequestDto
    {
        public long ProjectCourseId { get; set; }
        public string GroupCode { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string Status { get; set; } = "Active";
    }

    public class UpdateProjectGroupRequestDto
    {
        public string GroupCode { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
