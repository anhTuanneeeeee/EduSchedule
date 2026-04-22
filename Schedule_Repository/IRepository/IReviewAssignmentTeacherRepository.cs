using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.Repository
{
    public interface IReviewAssignmentTeacherRepository
    {
        Task<List<ReviewAssignmentTeacher>> GetBySemesterAsync(long semesterId);
        Task<bool> HasTeacherConflictSameSlotAsync(long teacherId, DateOnly assignedDate, long timeSlotId);
        Task<int> CountTeacherAssignmentsOnDateAsync(long teacherId, DateOnly assignedDate);
    }
}
