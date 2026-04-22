using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public interface IReviewAssignmentRepository
    {
        Task<bool> ExistsByReviewRoundIdAsync(long reviewRoundId);
        Task<ReviewAssignment> CreateWithTeachersAsync(ReviewAssignment assignment, List<ReviewAssignmentTeacher> teachers);
    }
}
