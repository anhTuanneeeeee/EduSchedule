using Schedule_Service.DTOs;

namespace Schedule_Service.IService;

public interface ITeacherService
{
    Task<List<TimeSlotResponse>> GetTimeSlotsAsync();
    Task<List<AvailabilityResponse>> GetTeacherAvailabilityAsync(long userId, DateOnly date);
    Task<bool> SubmitAvailabilityAsync(long userId, AvailabilityRequest request);
}
