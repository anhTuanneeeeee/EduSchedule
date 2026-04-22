using Schedule_Repository.Models;

namespace Schedule_Repository.IRepository;

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(long teacherId);
    Task<List<TimeSlot>> GetAllTimeSlotsAsync();
    Task<List<TeacherAvailability>> GetAvailabilityAsync(long teacherId, DateOnly date);
    Task SaveAvailabilityAsync(long teacherId, DateOnly date, List<long> timeSlotIds, string? note);
    Task<Teacher> CreateTeacherProfileAsync(Teacher teacher);
    Task<Teacher?> GetByUserIdAsync(long userId);
    Task<Teacher?> GetByTeacherCodeAsync(string teacherCode);
    Task<List<Teacher>> GetAvailableForReviewAsync();
    Task<List<Teacher>> GetByIdsAsync(List<long> teacherIds);
}
