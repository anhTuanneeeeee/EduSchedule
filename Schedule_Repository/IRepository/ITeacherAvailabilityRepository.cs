using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface ITeacherAvailabilityRepository
    {
        Task<List<TeacherAvailability>> GetByTeacherIdAsync(long teacherId, DateOnly? fromDate = null, DateOnly? toDate = null);
        Task<TeacherAvailability?> GetByIdAsync(long teacherAvailabilityId);
        Task<bool> ExistsDuplicateAsync(long teacherId, DateOnly availableDate, long timeSlotId, long? excludeId = null);
        Task<TeacherAvailability> CreateAsync(TeacherAvailability entity);
        Task<bool> UpdateAsync(TeacherAvailability entity);
        Task<bool> DeleteAsync(TeacherAvailability entity);
    }
}
