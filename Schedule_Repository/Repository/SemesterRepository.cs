using Microsoft.EntityFrameworkCore;
using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ScheduleForTeacherContext _context;

        public SemesterRepository(ScheduleForTeacherContext context)
        {
            _context = context;
        }

        public async Task<List<Semester>> GetAllAsync()
        {
            return await _context.Semesters
                .AsNoTracking()
                .OrderByDescending(x => x.SemesterId)
                .ToListAsync();
        }

        public async Task<Semester?> GetByIdAsync(long semesterId)
        {
            return await _context.Semesters
                .FirstOrDefaultAsync(x => x.SemesterId == semesterId);
        }

        public async Task<bool> SemesterCodeExistsAsync(string semesterCode, long? excludeSemesterId = null)
        {
            return await _context.Semesters.AnyAsync(x =>
                x.SemesterCode == semesterCode &&
                (!excludeSemesterId.HasValue || x.SemesterId != excludeSemesterId.Value));
        }

        public async Task<bool> SemesterNameExistsAsync(string semesterName, long? excludeSemesterId = null)
        {
            return await _context.Semesters.AnyAsync(x =>
                x.SemesterName == semesterName &&
                (!excludeSemesterId.HasValue || x.SemesterId != excludeSemesterId.Value));
        }

        public async Task<bool> HasProjectCoursesAsync(long semesterId)
        {
            return await _context.ProjectCourses.AnyAsync(x => x.SemesterId == semesterId);
        }

        public async Task<Semester> CreateAsync(Semester semester)
        {
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<bool> UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Semester semester)
        {
            _context.Semesters.Remove(semester);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
