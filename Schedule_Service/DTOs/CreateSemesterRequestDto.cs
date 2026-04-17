using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.DTOs
{
    public class CreateSemesterRequestDto
    {
        public string SemesterCode { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public int TermNumber { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
