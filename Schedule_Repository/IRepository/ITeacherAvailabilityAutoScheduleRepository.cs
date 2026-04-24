using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public interface ITeacherAvailabilityAutoScheduleRepository
    {
        Task<List<TeacherAvailability>> GetAvailableInRangeAsync(
            DateOnly fromDate,
            DateOnly toDate,
            List<long>? timeSlotIds = null);
    }
}
