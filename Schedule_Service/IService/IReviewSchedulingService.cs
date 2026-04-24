using Schedule_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Service.Service
{
    public interface IReviewSchedulingService
    {
        Task<(bool Success, string Message, AutoScheduleResultDto? Data)> AutoScheduleAsync(
            long assignedByUserId,
            AutoScheduleReviewAssignmentsRequestDto request);

        Task<(bool Success, string Message, CreatedReviewAssignmentResponseDto? Data)> ManualScheduleAsync(
            long assignedByUserId,
            ManualScheduleReviewAssignmentRequestDto request);

        Task<(bool Success, string Message)> UpdateAssignmentAsync(
            long reviewAssignmentId,
            UpdateReviewAssignmentRequestDto request);
    }
}