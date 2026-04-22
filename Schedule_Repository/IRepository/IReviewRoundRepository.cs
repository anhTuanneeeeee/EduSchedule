using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Schedule_Repository.Models;

namespace Schedule_Repository.Repository
{
    public interface IReviewRoundRepository
    {
        Task<List<ReviewRound>> GetUnscheduledBySemesterAsync(long semesterId);
        Task<ReviewRound?> GetByIdWithDetailsAsync(long reviewRoundId);
    }
}
