using System;

namespace Schedule_Service.DTOs
{
    public class TeacherDetailDto
    {
        public long TeacherId { get; set; }
        public string TeacherCode { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
    }
}
