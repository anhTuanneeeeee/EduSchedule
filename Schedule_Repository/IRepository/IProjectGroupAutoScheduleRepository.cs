using Schedule_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Repository.IRepository
{
    public interface IProjectGroupAutoScheduleRepository
    {
        Task<List<ProjectGroup>> GetBySemesterAsync(long semesterId);
    }
}
