using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public interface IReviewAssignmentQueryService
    {
        Task<(bool Success, string Message, List<ScheduleOverviewDateDto>? Data)> GetScheduleOverviewAsync(long semesterId);
    }
}