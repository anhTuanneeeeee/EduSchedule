using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public interface IReviewAssignmentQueryRepository
    {
        Task<List<ReviewAssignment>> GetScheduleOverviewAsync(
            long semesterId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            string? status = null);

        Task<List<ReviewAssignment>> GetMyScheduleAsync(long teacherId, DateOnly? fromDate = null, DateOnly? toDate = null);
    }
}
