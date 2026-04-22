using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface ITimeSlotRepository
    {
        Task<TimeSlot?> GetByIdAsync(long timeSlotId);
        Task<List<TimeSlot>> GetActiveAsync(List<long>? timeSlotIds = null);
    }
}
