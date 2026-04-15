using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using Schedule_Service.DTOs;
using Schedule_Service.IService;

namespace Schedule_Service.Service;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;

    public TeacherService(ITeacherRepository teacherRepository, IUserRepository userRepository)
    {
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TimeSlotResponse>> GetTimeSlotsAsync()
    {
        var slots = await _teacherRepository.GetAllTimeSlotsAsync();
        return slots.Select(s => new TimeSlotResponse
        {
            TimeSlotId = s.TimeSlotId,
            SlotNumber = s.SlotNumber,
            SlotName = s.SlotName,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
    }

    public async Task<List<AvailabilityResponse>> GetTeacherAvailabilityAsync(long userId, DateOnly date)
    {
        var teacher = await _userRepository.GetTeacherByUserIdAsync(userId);
        if (teacher == null) return new List<AvailabilityResponse>();

        var availabilities = await _teacherRepository.GetAvailabilityAsync(teacher.TeacherId, date);
        return availabilities.Select(a => new AvailabilityResponse
        {
            AvailabilityId = a.TeacherAvailabilityId,
            TeacherId = a.TeacherId,
            Date = a.AvailableDate,
            TimeSlotId = a.TimeSlotId,
            SlotName = a.TimeSlot.SlotName,
            Status = a.AvailabilityStatus,
            Note = a.Note
        }).ToList();
    }

    public async Task<bool> SubmitAvailabilityAsync(long userId, AvailabilityRequest request)
    {
        var teacher = await _userRepository.GetTeacherByUserIdAsync(userId);
        if (teacher == null) return false;

        await _teacherRepository.SaveAvailabilityAsync(teacher.TeacherId, request.Date, request.TimeSlotIds, request.Note);
        return true;
    }
}
