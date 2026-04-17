using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface ISemesterRepository
    {
        Task<List<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(long semesterId);

        Task<bool> SemesterCodeExistsAsync(string semesterCode, long? excludeSemesterId = null);
        Task<bool> SemesterNameExistsAsync(string semesterName, long? excludeSemesterId = null);
        Task<bool> HasProjectCoursesAsync(long semesterId);

        Task<Semester> CreateAsync(Semester semester);
        Task<bool> UpdateAsync(Semester semester);
        Task<bool> DeleteAsync(Semester semester);
    }
}
