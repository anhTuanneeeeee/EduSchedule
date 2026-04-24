using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public interface IReviewAutoSchedulingService
    {
        Task<(bool Success, string Message, AutoScheduleBySemesterResultDto? Data)> AutoScheduleBySemesterAsync(
            long assignedByUserId,
            AutoScheduleBySemesterRequestDto request);
    }
}