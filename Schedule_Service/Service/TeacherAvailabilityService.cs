using Schedule_Repository.IRepository;
using Schedule_Repository.Models;
using Schedule_Service.DTOs;
using Schedule_Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public class TeacherAvailabilityService : ITeacherAvailabilityService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITeacherAvailabilityRepository _teacherAvailabilityRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TeacherAvailabilityService(
            ITeacherRepository teacherRepository,
            ITeacherAvailabilityRepository teacherAvailabilityRepository,
            ITimeSlotRepository timeSlotRepository)
        {
            _teacherRepository = teacherRepository;
            _teacherAvailabilityRepository = teacherAvailabilityRepository;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<(bool Success, string Message, List<TeacherAvailabilityResponseDto>? Data)> GetMyAsync(
            long userId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
                return (false, "User hiện tại không phải giảng viên.", null);

            var items = await _teacherAvailabilityRepository.GetByTeacherIdAsync(
                teacher.TeacherId,
                fromDate,
                toDate);

            var result = items.Select(MapToDto).ToList();

            return (true, "Lấy danh sách lịch rảnh thành công.", result);
        }
        public async Task<(bool Success, string Message, List<TeacherAvailabilityResponseDto>? Data)> GetByTeacherCodeAsync(
            string teacherCode,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            if (string.IsNullOrWhiteSpace(teacherCode))
                return (false, "TeacherCode không được để trống.", null);

            var teacher = await _teacherRepository.GetByTeacherCodeAsync(teacherCode.Trim());
            if (teacher == null)
                return (false, "Không tìm thấy giảng viên với TeacherCode này.", null);

            var items = await _teacherAvailabilityRepository.GetByTeacherIdAsync(
                teacher.TeacherId,
                fromDate,
                toDate);

            return (true, "Lấy danh sách slot đã khai báo thành công.", items.Select(MapToDto).ToList());
        }

        public async Task<(bool Success, string Message, TeacherAvailabilityResponseDto? Data)> CreateAsync(
            long userId,
            CreateTeacherAvailabilityRequestDto request)
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
                return (false, "User hiện tại không phải giảng viên.", null);

            var timeSlot = await _timeSlotRepository.GetByIdAsync(request.TimeSlotId);
            if (timeSlot == null || !timeSlot.IsActive)
                return (false, "TimeSlot không tồn tại hoặc đang bị khóa.", null);

            var normalizedStatus = NormalizeAvailabilityStatus(request.AvailabilityStatus);
            if (normalizedStatus == null)
                return (false, "AvailabilityStatus chỉ chấp nhận AVAILABLE, UNAVAILABLE hoặc PREFERRED.", null);

            var duplicated = await _teacherAvailabilityRepository.ExistsDuplicateAsync(
                teacher.TeacherId,
                request.AvailableDate,
                request.TimeSlotId);

            if (duplicated)
                return (false, "Bạn đã khai báo slot này trong ngày này rồi.", null);

            var entity = new TeacherAvailability
            {
                TeacherId = teacher.TeacherId,
                AvailableDate = request.AvailableDate,
                TimeSlotId = request.TimeSlotId,
                AvailabilityStatus = normalizedStatus,
                Note = request.Note
            };

            var created = await _teacherAvailabilityRepository.CreateAsync(entity);
            var createdWithSlot = await _teacherAvailabilityRepository.GetByIdAsync(created.TeacherAvailabilityId);

            return (true, "Tạo lịch rảnh thành công.", MapToDto(createdWithSlot!));
        }

        public async Task<(bool Success, string Message)> UpdateAsync(
            long userId,
            long teacherAvailabilityId,
            UpdateTeacherAvailabilityRequestDto request)
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
                return (false, "User hiện tại không phải giảng viên.");

            var entity = await _teacherAvailabilityRepository.GetByIdAsync(teacherAvailabilityId);
            if (entity == null)
                return (false, "Không tìm thấy lịch rảnh.");

            if (entity.TeacherId != teacher.TeacherId)
                return (false, "Bạn không có quyền sửa lịch rảnh này.");

            var timeSlot = await _timeSlotRepository.GetByIdAsync(request.TimeSlotId);
            if (timeSlot == null || !timeSlot.IsActive)
                return (false, "TimeSlot không tồn tại hoặc đang bị khóa.");

            var normalizedStatus = NormalizeAvailabilityStatus(request.AvailabilityStatus);
            if (normalizedStatus == null)
                return (false, "AvailabilityStatus chỉ chấp nhận AVAILABLE, UNAVAILABLE hoặc PREFERRED.");

            var duplicated = await _teacherAvailabilityRepository.ExistsDuplicateAsync(
                teacher.TeacherId,
                request.AvailableDate,
                request.TimeSlotId,
                teacherAvailabilityId);

            if (duplicated)
                return (false, "Đã tồn tại lịch rảnh khác cùng ngày và slot.");

            entity.AvailableDate = request.AvailableDate;
            entity.TimeSlotId = request.TimeSlotId;
            entity.AvailabilityStatus = normalizedStatus;
            entity.Note = request.Note;

            var result = await _teacherAvailabilityRepository.UpdateAsync(entity);

            return result
                ? (true, "Cập nhật lịch rảnh thành công.")
                : (false, "Cập nhật lịch rảnh thất bại.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(
            long userId,
            long teacherAvailabilityId)
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            if (teacher == null)
                return (false, "User hiện tại không phải giảng viên.");

            var entity = await _teacherAvailabilityRepository.GetByIdAsync(teacherAvailabilityId);
            if (entity == null)
                return (false, "Không tìm thấy lịch rảnh.");

            if (entity.TeacherId != teacher.TeacherId)
                return (false, "Bạn không có quyền xóa lịch rảnh này.");

            var result = await _teacherAvailabilityRepository.DeleteAsync(entity);

            return result
                ? (true, "Xóa lịch rảnh thành công.")
                : (false, "Xóa lịch rảnh thất bại.");
        }

        public async Task<List<TeacherDetailDto>> GetAvailableOnSlotAsync(DateOnly date, long slotId)
        {
            var availabilities = await _teacherAvailabilityRepository.GetAvailableInRangeAsync(
                date, date, new List<long> { slotId });

            return availabilities.Select(a => new TeacherDetailDto
            {
                TeacherId = a.TeacherId,
                TeacherCode = a.Teacher.TeacherCode,
                FullName = a.Teacher.User.FullName,
                Email = a.Teacher.User.Email,
                Phone = a.Teacher.User.Phone,
                Department = a.Teacher.Department
            }).ToList();
        }

        private static string? NormalizeAvailabilityStatus(string? status)
        {
            var normalized = status?.Trim().ToUpperInvariant();

            return normalized is "AVAILABLE" or "UNAVAILABLE" or "PREFERRED"
                ? normalized
                : null;
        }

        private static TeacherAvailabilityResponseDto MapToDto(TeacherAvailability entity)
        {
            return new TeacherAvailabilityResponseDto
            {
                TeacherAvailabilityId = entity.TeacherAvailabilityId,
                TeacherId = entity.TeacherId,
                AvailableDate = entity.AvailableDate,
                TimeSlotId = entity.TimeSlotId,
                SlotNumber = entity.TimeSlot.SlotNumber,
                SlotName = entity.TimeSlot.SlotName,
                StartTime = entity.TimeSlot.StartTime,
                EndTime = entity.TimeSlot.EndTime,
                AvailabilityStatus = entity.AvailabilityStatus,
                Note = entity.Note
            };
        }
    }
}
