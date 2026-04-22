using Microsoft.EntityFrameworkCore;
using Schedule_Repository.Models;
using Schedule_Repository.IRepository;

namespace Schedule_Repository.Repository;

public class TeacherRepository : ITeacherRepository
{
    private readonly ScheduleForTeacherContext _context;

    public TeacherRepository(ScheduleForTeacherContext context)
    {
        _context = context;
    }

    public async Task<Teacher?> GetByIdAsync(long teacherId)
    {
        return await _context.Teachers
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
    }

    public async Task<List<TimeSlot>> GetAllTimeSlotsAsync()
    {
        return await _context.TimeSlots
            .Where(ts => ts.IsActive)
            .OrderBy(ts => ts.SlotNumber)
            .ToListAsync();
    }

    public async Task<List<TeacherAvailability>> GetAvailabilityAsync(long teacherId, DateOnly date)
    {
        return await _context.TeacherAvailabilities
            .Include(ta => ta.TimeSlot)
            .Where(ta => ta.TeacherId == teacherId && ta.AvailableDate == date)
            .ToListAsync();
    }

    public async Task SaveAvailabilityAsync(long teacherId, DateOnly date, List<long> timeSlotIds, string? note)
    {
        // Remove existing availability for that date to overwrite (typical for "updating" free slots)
        var existing = await _context.TeacherAvailabilities
            .Where(ta => ta.TeacherId == teacherId && ta.AvailableDate == date)
            .ToListAsync();
        
        _context.TeacherAvailabilities.RemoveRange(existing);

        // Add new ones
        foreach (var slotId in timeSlotIds)
        {
            _context.TeacherAvailabilities.Add(new TeacherAvailability
            {
                TeacherId = teacherId,
                AvailableDate = date,
                TimeSlotId = slotId,
                AvailabilityStatus = "AVAILABLE",
                Note = note
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Teacher> CreateTeacherProfileAsync(Teacher teacher)
    {
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        return teacher;
    }
    public async Task<Teacher?> GetByUserIdAsync(long userId)
    {
        return await _context.Teachers
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }
    public async Task<Teacher?> GetByTeacherCodeAsync(string teacherCode)
    {
        return await _context.Teachers
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TeacherCode == teacherCode);
    }
}
